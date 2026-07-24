#!/bin/sh
set -e

cd "$(dirname "$0")/.."

COMPOSE_FILE="docker-compose.k6.yml"

cleanup() {
    docker compose -f "$COMPOSE_FILE" down -v
}
trap cleanup EXIT

docker compose -f "$COMPOSE_FILE" up --build --abort-on-container-exit --exit-code-from k6
