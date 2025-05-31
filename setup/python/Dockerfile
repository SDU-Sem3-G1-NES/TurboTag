FROM python:3.10-slim

ENV DEBIAN_FRONTEND=noninteractive
ENV PIP_DEFAULT_TIMEOUT=300

RUN apt-get update && \
    apt-get install -y curl && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir --timeout=300 -r requirements.txt -i https://pypi.org/simple

COPY . .

CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000", "--reload"]
