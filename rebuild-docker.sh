#!/bin/bash

set -e
set -x

for arg in "$@"; do
  if [ "$arg" == "-h" ] || [ "$arg" == "--hard" ]; then
    if [ "$EUID" -ne 0 ]; then
      echo "Please run as root when using -h/--hard"
      exit
    fi
    
    docker network prune -f
    systemctl stop docker
    iptables -t nat -F
    ip link set docker0 down
    ip link delete docker0
    systemctl restart docker
  fi
done

docker-compose down

for arg in "$@"; do
  if [ "$arg" == "mongo" ] || [ "$arg" == "all" ]; then
    docker rmi turbotag-mongo || true
    docker volume rm turbotag_mongo_data || true
  fi
  
  if [ "$arg" == "postgres" ] || [ "$arg" == "all" ]; then
    docker rmi turbotag-postgres || true
    docker volume rm turbotag_postgres_data || true
  fi
done

docker compose build --no-cache --progress plain
docker compose up -d