from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from pathlib import Path
import requests
import csv
import os

app = FastAPI(
    title="Content Generator API (Ollama)",
    description="Generates tags (from a fixed CSV list) and descriptions using the Ollama gemma:2b model.",
    version="2.0.0"
)

# Ollama endpoint inside Docker
OLLAMA_URL = "http://ollama:11434/api/generate"

BASE_DIR = Path(__file__).resolve().parent
TAG_CSV_PATH = os.getenv("TAG_CSV_PATH", str(BASE_DIR / "tags.csv"))

try:
    with open(TAG_CSV_PATH, newline="", encoding="utf-8") as f:
        reader = csv.reader(f)
        allowed_tags = [tag.strip() for tag in next(reader) if tag.strip()]
except Exception as e:
    raise RuntimeError(f"Could not load tags from {TAG_CSV_PATH!r}: {e}")

class GenerationRequest(BaseModel):
    text: str

class GenerationResponse(BaseModel):
    tags: str
    description: str

def call_ollama(prompt: str, model: str = "gemma:2b") -> str:
    payload = {"model": model, "prompt": prompt, "stream": False}
    try:
        resp = requests.post(OLLAMA_URL, json=payload, timeout=300)
        resp.raise_for_status()
        return resp.json().get("response", "").strip()
    except requests.RequestException as exc:
        raise HTTPException(status_code=500, detail="AI generation failed")

@app.post("/generate-content", response_model=GenerationResponse, tags=["Generation"])
def generate_content(req: GenerationRequest):
    text = req.text.strip()
    allowed_list_str = ", ".join(allowed_tags)

    # Prompt for tags
    tag_prompt = f"""
Allowed tags:
{allowed_list_str}

Return exactly 5 tags from the list above that best match the input.
Output a comma-separated list only.

Input:
{text}

Tags:""".strip()

    # Prompt for description
    desc_prompt = f"""
Return one concise sentence describing the input.
Output only that sentence.

Input:
{text}

Description:""".strip()

    raw_tags = call_ollama(tag_prompt)
    raw_desc = call_ollama(desc_prompt)

    # Clean and enforce allowed-tags constraint
    candidates = [
        tag.strip()
        for tag in raw_tags.replace("\n", ",").split(",")
        if tag.strip()
    ]

    final_tags = []
    for tag in candidates:
        if tag in allowed_tags and tag not in final_tags:
            final_tags.append(tag)
        if len(final_tags) == 5:
            break

    # Pad with additional allowed tags if fewer than 5
    for tag in allowed_tags:
        if len(final_tags) == 5:
            break
        if tag not in final_tags:
            final_tags.append(tag)

    return GenerationResponse(
        tags=", ".join(final_tags),
        description=raw_desc
    )
