from fastapi import FastAPI
from pydantic import BaseModel
import requests
import csv
from pathlib import Path
import os
from sentence_transformers import SentenceTransformer, util
from typing import List

app = FastAPI()

OLLAMA_URL = "http://ollama:11434/api/generate"
BACKEND_CALLBACK_URL = "http://host.docker.internal:5088/Lesson/CompleteGeneration"

BASE_DIR = Path(__file__).resolve().parent
TAG_CSV_PATH = os.getenv("TAG_CSV_PATH", str(BASE_DIR / "tags.csv"))



embedding_model = SentenceTransformer("all-MiniLM-L6-v2")

class AsyncGenerationRequest(BaseModel):
    uploadId: int
    text: str

def call_ollama(prompt: str, model: str = "gemma:2b") -> str:
    payload = {"model": model, "prompt": prompt, "stream": False}
    resp = requests.post(OLLAMA_URL, json=payload, timeout=300)
    resp.raise_for_status()
    return resp.json().get("response", "").strip()

def get_top_tags_by_similarity(text: str, tags: List[str], top_n=5) -> List[str]:
    input_emb = embedding_model.encode(text, convert_to_tensor=True)
    tag_embs = embedding_model.encode(tags, convert_to_tensor=True)
    scores = util.cos_sim(input_emb, tag_embs)[0]
    ranked = sorted(zip(tags, scores), key=lambda x: x[1], reverse=True)
    return [tag for tag, _ in ranked[:top_n]]

@app.post("/generate-async")
def generate_async(req: AsyncGenerationRequest):
    try:
        with open(TAG_CSV_PATH, newline="", encoding="utf-8") as f:
            reader = csv.DictReader(f)
            allowed_tags = [row["Tag Name"].strip() for row in reader if row.get("Tag Name")]
    except Exception as e:
        raise RuntimeError(f"Could not load tags from {TAG_CSV_PATH!r}: {e}")
    
    text = req.text.strip()
    upload_id = req.uploadId

    desc_prompt = f"""
Based solely on the input below, write exactly one clear, factual sentence summarizing it.
Do not introduce filler, explanations, or any extra information.
Output only that sentence.

Input:
{text}

Description:""".strip()
    raw_desc = call_ollama(desc_prompt)
    description = raw_desc.split(":", 1)[1].strip() if ":" in raw_desc else raw_desc.strip()

    final_tags = get_top_tags_by_similarity(text, allowed_tags, top_n=5)
    tags = ", ".join(final_tags)

    payload = {
        "uploadId": upload_id,
        "description": description,
        "tags": tags
    }

    try:
        response = requests.post(BACKEND_CALLBACK_URL, json=payload)
        response.raise_for_status()
    except Exception as e:
        print(f"Callback to backend failed: {e}")

    return {"status": "done"}
