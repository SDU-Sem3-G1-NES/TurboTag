from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import requests
import csv
from pathlib import Path
import os
import logging
from typing import List
import numpy as np
from sentence_transformers import SentenceTransformer, util
import urllib3

# Disable SSL warnings for development purposes
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)


app = FastAPI()

# ─── Logging setup ──────────────────────────────────────────────────────────────
logging.basicConfig(level=logging.DEBUG)
logger = logging.getLogger("main")

# ─── Config ────────────────────────────────────────────────────────────────────
OLLAMA_URL = "http://ollama:11434/api/generate"
BACKEND_CALLBACK_URL = "http://host.docker.internal:5088/Lesson/CompleteGeneration"
DESCRIPTION_MODEL = "gemma:2b"
EMBEDDING_MODEL = SentenceTransformer("all-MiniLM-L6-v2")
BASE_DIR = Path(__file__).resolve().parent
TAG_CSV_PATH = os.getenv("TAG_CSV_PATH", str(BASE_DIR / "tags.csv"))

# Preload allowed tags and their embeddings
with open(TAG_CSV_PATH, newline="", encoding="utf-8") as f:
    reader = csv.DictReader(f)
    ALLOWED_TAGS = [row["Tag Name"].strip() for row in reader if row.get("Tag Name")]
ALLOWED_EMBS = EMBEDDING_MODEL.encode(ALLOWED_TAGS, normalize_embeddings=True)

# thresholds & params
SIM_THRESHOLD = 0.70   
MIN_TAGS = 3         

# ─── Request DTO ───────────────────────────────────────────────────────────────
class AsyncGenerationRequest(BaseModel):
    uploadId: int
    text: str

# ─── Ollama call ───────────────────────────────────────────────────────────────
def call_ollama(prompt: str, model: str) -> str:
    resp = requests.post(OLLAMA_URL, json={"model": model, "prompt":prompt, "stream":False}, timeout=300)
    resp.raise_for_status()
    return resp.json().get("response", "").strip()

# ─── Generate Description ─────────────────────────────────────────────────────
def generate_description(text: str) -> str:
    prompt = f"""
Write a 1-sentence summary describing the purpose of this music lesson. It should focus on the skill level and learning objective (e.g., learning chords, playing a song, etc.)

Lesson:
{text}

Summary:
""".strip()
    try:
        out = call_ollama(prompt, DESCRIPTION_MODEL)
        logger.debug("[DESCRIPTION_RAW] %s", out)
        return out.split("\n",1)[-1].strip()
    except Exception as e:
        logger.error("Desc gen failed: %s", e)
        return ""

# ─── Generate Raw Tags ─────────────────────────────────────────────────────────
def generate_tags_from_text(text: str) -> List[str]:
    prompt = f"""
You are a smart assistant for music education. From the lesson text below, generate up to 7 concise tags (1–3 words each)
that capture:
 • Skills taught (e.g. chord switching, strumming)
 • Instrument(s)
 • Audience level (e.g. beginner)
 • Practice format (e.g. exercise, routine)
 • Song or artist names if mentioned

Avoid fret-numbers or finger positions. Do not repeat synonyms or broad+specific (e.g. don't list both "Guitar" and "Electric Guitar").
Lesson:
\"\"\"{text}\"\"\"

Tags:"""
    try:
        out = call_ollama(prompt, DESCRIPTION_MODEL)
        logger.debug("[GENERATED_TAGS_RAW]\n%s", out)
        lines = [l.strip() for l in out.replace("•","\n").splitlines() if l.strip()]
        tags = []
        for line in lines:
            t = line.lstrip("0123456789.-:)* ").strip()
            for part in t.split(","):
                p = part.strip().title()
                if p and p not in tags:
                    tags.append(p)
        return tags
    except Exception as e:
        logger.error("Tag gen failed: %s", e)
        return []

# ─── Match & Filter to Allowed ────────────────────────────────────────────────
def filter_tags(raw: List[str]) -> List[str]:
    matched = {}
    if not raw:
        return []

    raw_emb = EMBEDDING_MODEL.encode(raw, normalize_embeddings=True)
    sims = util.cos_sim(raw_emb, ALLOWED_EMBS).cpu().numpy()

    for i, rt in enumerate(raw):
        j = np.argmax(sims[i])
        score = float(sims[i][j])
        if score >= SIM_THRESHOLD:
            tag = ALLOWED_TAGS[j]
            if tag not in matched or matched[tag] < score:
                matched[tag] = score
            logger.debug("Match: %-20s → %-20s (%.3f)", rt, tag, score)

    result = sorted(matched.keys(), key=lambda t: -matched[t])
    return result

# ─── Drop Hierarchical Redundancy ──────────────────────────────────────────────
def prune_redundant(tags: List[str]) -> List[str]:
    t = tags.copy()
    if "Guitar" in t and any(x in t for x in ["Electric Guitar","Acoustic Guitar","Classical Guitar"]):
        t = [x for x in t if x!="Guitar"]
    if "Strings" in t and any(x in t for x in ["Guitar","Violin","Cello","Ukulele"]):
        t = [x for x in t if x!="Strings"]
    if "Percussion" in t and any(x in t for x in ["Drum Kit","Snare Drum","Xylophone"]):
        t = [x for x in t if x!="Percussion"]
    if "Music Video" in t:
        t = [x for x in t if x!="Music Video"]
    return t

# ─── Fallbacks: add instrument, level, or sem-expansion ────────────────────────
def apply_fallbacks(tags: List[str], text: str) -> List[str]:
    final = tags.copy()
    low = text.lower()
    # instrument hint
    for kw, tag in [("guitar","Guitar"),("piano","Piano"),("drum","Drum Kit"),("violin","Violin")]:
        if kw in low and tag not in final:
            final.append(tag)
            break
    # level hint
    for kw, tag in [("beginner","Beginner"),("intermediate","Intermediate"),("advanced","Advanced")]:
        if kw in low and tag not in final:
            final.append(tag)
            break
    # semantic expansion if too few
    if len(final) < MIN_TAGS:
        lesson_emb = EMBEDDING_MODEL.encode(text, normalize_embeddings=True)
        sims = np.dot(ALLOWED_EMBS, lesson_emb)
        for idx in np.argsort(-sims):
            cand = ALLOWED_TAGS[int(idx)]
            if cand not in final and sims[int(idx)]>0.50:
                # skip broad categories if specifics exist
                if cand in ["Strings","Percussion","Woodwinds","Brass"] and any(x in final for x in ALLOWED_TAGS):
                    continue
                final.append(cand)
                if len(final)>=MIN_TAGS:
                    break
    return final

# ─── Main Endpoint ────────────────────────────────────────────────────────────
@app.post("/generate-async")
def generate_async(req: AsyncGenerationRequest):
    text = req.text.strip()
    if not text:
        raise HTTPException(400, "Empty lesson text.")

    description = generate_description(text)

    raw_tags = generate_tags_from_text(text)

    filt = filter_tags(raw_tags)

    pruned = prune_redundant(filt)

    final = apply_fallbacks(pruned, text)

    final = final[:8]

    payload = {
        "uploadId": req.uploadId,
        "description": description,
        "tags": ", ".join(final)
    }

    try:
        resp = requests.post(BACKEND_CALLBACK_URL, json=payload, timeout=10, verify=False)
        resp.raise_for_status()
    except Exception as e:
        logger.error("Callback failed: %s", e)
        return {"status":"error", "detail":str(e)}

    return {"status":"done"}
