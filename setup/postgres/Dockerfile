FROM postgres:latest

COPY .env /app/.env
COPY blank.sql /docker-entrypoint-initdb.d/blank.sql

ENV POSTGRES_USER=${POSTGRES_USER}
ENV POSTGRES_PASSWORD=${POSTGRES_PASSWORD}
ENV POSTGRES_DB=postgres

EXPOSE 5432