from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from pathlib import Path
import requests
import csv
import os

from sentence_transformers import SentenceTransformer, util
from typing import List

app = FastAPI(
    title="Content Generator API (Ollama)",
    description="Generates tags (from CSV) and descriptions using Ollama + semantic similarity.",
    version="2.1.0"
)

OLLAMA_URL = "http://ollama:11434/api/generate"

BASE_DIR = Path(__file__).resolve().parent
TAG_CSV_PATH = os.getenv("TAG_CSV_PATH", str(BASE_DIR / "tags.csv"))

# Load allowed tags from CSV
try:
    with open(TAG_CSV_PATH, newline="", encoding="utf-8") as f:
        reader = csv.DictReader(f)
        allowed_tags = [row["Tag Name"].strip() for row in reader if row.get("Tag Name")]
        if not allowed_tags:
            raise ValueError("No tags found in CSV")
except Exception as e:
    raise RuntimeError(f"Could not load tags from {TAG_CSV_PATH!r}: {e}")


# Load transformer model once
embedding_model = SentenceTransformer("all-MiniLM-L6-v2")

class GenerationRequest(BaseModel):
    text: str

class GenerationResponse(BaseModel):
    tags: str
    description: str

def call_ollama(prompt: str, model: str = "gemma:2b") -> str:
    """Send prompt to Ollama and return response."""
    payload = {"model": model, "prompt": prompt, "stream": False}
    try:
        resp = requests.post(OLLAMA_URL, json=payload, timeout=300)
        resp.raise_for_status()
        return resp.json().get("response", "").strip()
    except requests.RequestException as e:
        raise HTTPException(status_code=500, detail=f"AI generation failed: {e}")

def get_top_tags_by_similarity(text: str, tags: List[str], top_n=5) -> List[str]:
    """Rank tags using semantic similarity and return top N."""
    input_emb = embedding_model.encode(text, convert_to_tensor=True)
    tag_embs = embedding_model.encode(tags, convert_to_tensor=True)
    scores = util.cos_sim(input_emb, tag_embs)[0]
    ranked = sorted(zip(tags, scores), key=lambda x: x[1], reverse=True)
    return [tag for tag, _ in ranked[:top_n]]


@app.post("/generate-content", response_model=GenerationResponse, tags=["Generation"])
def generate_content(req: GenerationRequest):
    text = req.text.strip()

    # Use AI to generate a short description
    desc_prompt = f"""
Based solely on the input below, write exactly one clear, factual sentence summarizing it.
Do not introduce filler, explanations, or any extra information.
Output only that sentence.

Input:
{text}

Description:""".strip()

    raw_desc = call_ollama(desc_prompt)

    # Clean description (remove possible "Description: ...")
    description = raw_desc.split(":", 1)[1].strip() if ":" in raw_desc else raw_desc.strip()

    # Use embeddings to find top 5 tags
    final_tags = get_top_tags_by_similarity(text, allowed_tags, top_n=5)

    return GenerationResponse(
        tags=", ".join(final_tags),
        description=description
    )