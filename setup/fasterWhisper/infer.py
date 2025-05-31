from fastapi import FastAPI, Body
from faster_whisper import WhisperModel
import os

app = FastAPI()
model = WhisperModel("tiny", device="cpu")

@app.post("/transcribe-paths/")
async def transcribe_paths(paths: list[str] = Body(...)):
    all_text = []
    for path in paths:
        if path.startswith("/tmp/"):
            container_path = path.replace("/tmp", "/host-tmp", 1)
            safe_root = "/host-tmp"
        elif path.startswith("%localappdata%/Temp"):
            container_path = path.replace("%localappdata%/Temp", "/win-tmp", 1)
            safe_root = "/win-tmp"
        else:
            continue 

        container_path = os.path.normpath(container_path)

        if not container_path.startswith(safe_root):
            continue
        if not os.path.exists(container_path):
            continue

        segments, _ = model.transcribe(container_path, word_timestamps=True)
        all_text.extend(segment.text for segment in segments)

    return " ".join(all_text)
