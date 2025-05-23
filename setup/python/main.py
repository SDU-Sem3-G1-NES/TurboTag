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

# Locate tags.csv next to this script, with optional env override
BASE_DIR = Path(__file__).resolve().parent
TAG_CSV_PATH = os.getenv("TAG_CSV_PATH", str(BASE_DIR / "tags.csv"))

# Load allowed tags once at startup (read the Tag Name column)
try:
    with open(TAG_CSV_PATH, newline="", encoding="utf-8") as f:
        reader = csv.DictReader(f)
        allowed_tags = [row["Tag Name"].strip() for row in reader if row.get("Tag Name")]
        if not allowed_tags:
            raise ValueError("No tags found in CSV")
except Exception as e:
    raise RuntimeError(f"Could not load tags from {TAG_CSV_PATH!r}: {e}")

class GenerationRequest(BaseModel):
    text: str

class GenerationResponse(BaseModel):
    tags: str
    description: str

def call_ollama(prompt: str, model: str = "gemma:2b") -> str:
    """Send a prompt to Ollama and return its raw response."""
    payload = {"model": model, "prompt": prompt, "stream": False}
    try:
        resp = requests.post(OLLAMA_URL, json=payload)
        resp.raise_for_status()
        return resp.json().get("response", "").strip()
    except requests.RequestException:
        raise HTTPException(status_code=500, detail="AI generation failed")

@app.post("/generate-content", response_model=GenerationResponse, tags=["Generation"])
def generate_content(req: GenerationRequest):
    text = req.text.strip()
    allowed_list_str = ", ".join(allowed_tags)

    # Prompt for tags
    tag_prompt = f"""
You are a tagging assistant.

Given a list of allowed tags and an input text, return the 5 tags from the allowed list that are the most semantically relevant to the content. The tags must be chosen only from the allowed list—do not make up or rephrase any.

Only output a single comma-separated list of exactly 5 tags. No commentary, no bullet points, no numbering.

Allowed tags: {allowed_list_str}
Input: {text}
Tags:""".strip()


# Prompt for description
    desc_prompt = f"""
Based solely on the input below, write exactly one clear, factual sentence summarizing it.
Do not introduce filler, explanations, or any extra information.
Output only that sentence.

Input:
{text}

Description:""".strip()

    # Generate raw outputs
    raw_tags = call_ollama(tag_prompt)
    raw_desc = call_ollama(desc_prompt)

    # Strip anything before the first colon in the description
    if ":" in raw_desc:
        description = raw_desc.split(":", 1)[1].strip()
    else:
        description = raw_desc.strip()

    # Clean & enforce allowed-tags constraint
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

    # Pad with additional tags if needed
    for tag in allowed_tags:
        if len(final_tags) == 5:
            break
        if tag not in final_tags:
            final_tags.append(tag)

    return GenerationResponse(
        tags=", ".join(final_tags),
        description=description
    )