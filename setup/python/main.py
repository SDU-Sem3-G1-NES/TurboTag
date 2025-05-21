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
        response = requests.post(OLLAMA_URL, json=payload, timeout=300)
        response.raise_for_status()
        return response.json()["response"].strip()
    except requests.RequestException as e:
        print(f"❌ Error calling Ollama: {e}")
        raise HTTPException(status_code=500, detail="Failed to generate content using AI model.")

@app.post("/generate-content", response_model=GenerationResponse, tags=["Generation"])
def generate_content(req: GenerationRequest):
    """
    Generate 5 relevant tags (comma-separated) and a clean description using Ollama gemma:2b model.
    """
    text = req.text.strip()

    tag_prompt = f"""You are an API that returns exactly 5 most relevant tags from the input text.
Respond ONLY with a comma-separated list of tags, no bullet points, newlines, or explanations.

Input text:
{text}

Tags:"""

    desc_prompt = f"""You are an API that returns a plain, concise, and clear one-sentence description of the input text.
Respond ONLY with the description, no explanations or formatting.

Input text:
{text}

Description:"""

    raw_tags = call_ollama(tag_prompt)
    raw_description = call_ollama(desc_prompt)

    cleaned_tags = ', '.join(
        [tag.strip().lstrip("-").strip() for tag in raw_tags.replace("\n", ",").split(",") if tag.strip()]
    )
    tag_list = list(dict.fromkeys(cleaned_tags.split(", ")))
    tag_list = tag_list[:5]  
    final_tags = ', '.join(tag_list)

    final_description = raw_description.strip()

    return GenerationResponse(tags=final_tags, description=final_description)