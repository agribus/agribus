#!/bin/bash

export PATH="/usr/bin:/bin:/usr/local/bin"

set -a
source ../.env
set +a

cd $PROJECT_PATH || exit 1

./git_pull_prod.sh

docker compose -f ./docker/compose.preprod.yaml build --no-cache lychee

docker compose -f ./docker/compose.preprod.yaml --env-file .env up -d
