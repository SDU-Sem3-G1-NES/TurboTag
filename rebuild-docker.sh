#!/bin/bash

set -e
set -x

rebuild=0

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

docker compose down

for arg in "$@"; do
  if [ "$arg" == "-r" ] || [ "$arg" == "--rebuild" ]; then
    rebuild=1
  fi
  
  if [ "$arg" == "mongo" ] || [ "$arg" == "all" ]; then
    docker rmi turbotag-mongo || true
    docker volume rm turbotag_mongo_data || true
    rebuild=1
  fi
  
  if [ "$arg" == "postgres" ] || [ "$arg" == "all" ]; then
    docker rmi turbotag-postgres || true
    docker volume rm turbotag_postgres_data || true
    rebuild=1
  fi
done

if [ $rebuild -eq 1 ]; then
  docker compose build --no-cache --progress plain
fi

docker compose up -d