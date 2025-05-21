from fastapi import FastAPI, Body
from faster_whisper import WhisperModel
import os

app = FastAPI()
model = WhisperModel("tiny", device="cpu")

@app.post("/transcribe-paths/")
async def transcribe_paths(paths: list[str] = Body(...)):
    all_text = []
    for path in paths:
        container_path = path.replace("/tmp", "/host-tmp", 1)
        container_path = os.path.normpath(container_path)
        safe_root = "/host-tmp"
        if not container_path.startswith(safe_root):
            continue
        if not os.path.exists(container_path):
            continue
        segments, info = model.transcribe(container_path, word_timestamps=True)
        all_text.extend(segment.text for segment in segments)
    return " ".join(all_text)