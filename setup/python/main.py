from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import requests

app = FastAPI(
    title="Content Generator API (Ollama)",
    description="Generates tags and descriptions using the Ollama gemma:2b model.",
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

    tag_prompt = f"""You are an API that only returns a plain, comma-separated list of relevant tags based on the input text. 
    Do not include any introductory phrases, explanations, or formatting—just the tags.
    
    Input text:
    {text}
    
    Output tags:"""
    
    desc_prompt = f"""You are an API that only returns a plain, concise, and clear description of the input text. 
    Do not include any introductory phrases, explanations, or formatting—just the description.
    
    Input text:
    {text}
    
    Output description:"""

    tags = call_ollama(tag_prompt)
    description = call_ollama(desc_prompt)

    return GenerationResponse(tags=tags, description=description)
