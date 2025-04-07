FROM mongo:latest
WORKDIR /docker-entrypoint-initdb.d
RUN apt-get update -y && apt-get install -y nodejs npm

COPY mongo-init.js /docker-entrypoint-initdb.d/mongo-init.js
COPY mongod.conf /etc/mongod.conf

RUN npm install mongodb

RUN ulimit -n 64000

EXPOSE 27017
CMD ["mongod", "--config", "/etc/mongod.conf"]