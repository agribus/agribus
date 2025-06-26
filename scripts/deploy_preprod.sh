#!/bin/bash

export PATH="/usr/bin:/bin:/usr/local/bin"

cd /srv/agribus || exit 1

docker compose -f ./docker/compose.preprod.yaml build --no-cache lychee

docker compose -f ./docker/compose.preprod.yaml --env-file .env up -d
