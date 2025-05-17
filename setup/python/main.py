from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import requests

app = FastAPI(
    title="Content Generator API (Ollama)",
    description="Generates tags and descriptions using the Ollama gemma:4b model.",
    version="2.0.0"
)

# URL to reach Ollama in Docker
OLLAMA_URL = "http://ollama:11434/api/generate"
# For local testing outside Docker: use "http://localhost:11434/api/generate"

class GenerationRequest(BaseModel):
    text: str

class GenerationResponse(BaseModel):
    tags: str
    description: str

def call_ollama(prompt: str, model: str = "gemma:2b") -> str:
    """Call Ollama API with a prompt and return the response."""
    payload = {
        "model": model,
        "prompt": prompt,
        "stream": False
    }

    try:
        response = requests.post(OLLAMA_URL, json=payload, timeout=60)
        response.raise_for_status()
        return response.json()["response"].strip()
    except requests.RequestException as e:
        print(f"❌ Error calling Ollama: {e}")
        raise HTTPException(status_code=500, detail="Failed to generate content using AI model.")

@app.post("/generate-content", response_model=GenerationResponse, tags=["Generation"])
def generate_content(req: GenerationRequest):
    """
    Generate tags and description from input text using Ollama gemma:2b model.
    """
    text = req.text.strip()

    tag_prompt = f"Generate a comma-separated list of short, relevant tags for the following text:\n\n{text}\n\nTags:"
    desc_prompt = f"Write a concise, clear description for the following text:\n\n{text}\n\nDescription:"

    tags = call_ollama(tag_prompt)
    description = call_ollama(desc_prompt)

    return GenerationResponse(tags=tags, description=description)
