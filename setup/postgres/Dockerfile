FROM postgres:latest

COPY blank.sql /docker-entrypoint-initdb.d/blank.sql

ENV POSTGRES_DB=postgres

EXPOSE 5432