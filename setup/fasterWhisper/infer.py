from fastapi import FastAPI, Body
from faster_whisper import WhisperModel
import os

app = FastAPI()

model = WhisperModel("tiny", device="cpu")

local_appdata = os.environ.get("LOCALAPPDATA", "").replace("\\", "/")

@app.post("/transcribe-paths/")
async def transcribe_paths(paths: list[str] = Body(...)):
    all_text = []

    for path in paths:
        container_path = None
        safe_root = None

        # Handle Linux temp path
        if path.startswith("/tmp/"):
            container_path = path.replace("/tmp", "/host-tmp", 1)
            safe_root = "/host-tmp"

        # Handle Windows temp path
        elif local_appdata and path.startswith(f"{local_appdata}/Temp"):
            subpath = path.split("Temp", 1)[-1].lstrip("/\\")
            container_path = os.path.join("/win-tmp", subpath)
            safe_root = "/win-tmp"

        if container_path:
            container_path = os.path.normpath(container_path)

            if not container_path.startswith(safe_root):
                continue
            if not os.path.exists(container_path):
                continue

            segments, _ = model.transcribe(container_path, word_timestamps=True)
            all_text.extend(segment.text for segment in segments)

    return " ".join(all_text)
