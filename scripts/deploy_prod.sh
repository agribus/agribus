#!/bin/bash

export PATH="/usr/bin:/bin:/usr/local/bin"

cd /srv/agribus || exit 1

git fetch origin main
git checkout main
git pull origin main

docker compose -f ./docker/compose.prod.yaml build --no-cache lychee

docker compose -f ./docker/compose.prod.yaml --env-file .env up -d
