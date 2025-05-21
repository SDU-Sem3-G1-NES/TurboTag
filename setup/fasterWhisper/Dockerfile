FROM nvidia/cuda:12.3.2-cudnn9-runtime-ubuntu22.04

WORKDIR /app

RUN apt-get update -y && apt-get install -y python3-pip

COPY requirements.txt ./
RUN pip3 install -r requirements.txt

COPY infer.py ./

EXPOSE 8000

CMD ["uvicorn", "infer:app", "--host", "0.0.0.0", "--port", "8000"]