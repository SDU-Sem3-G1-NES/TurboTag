FROM mongo:latest
WORKDIR /docker-entrypoint-initdb.d
RUN apt-get update -y && apt-get install -y nodejs npm

COPY mongo-init.js /docker-entrypoint-initdb.d/mongo-init.js

RUN npm install mongodb

RUN ulimit -n 64000

ENV WAITFORIT_TIMEOUT=60

EXPOSE 27017
#CMD ["sh", "-c", "/docker-entrypoint-initdb.d/wait-for-it.sh localhost:27017 -- node /docker-entrypoint-initdb.d/mongo-init.js"]