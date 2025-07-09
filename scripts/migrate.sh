#!/bin/bash

# Export the env variable ConnectionStrings__Postgres to the environment
export ConnectionStrings__Postgres=$(grep -E '^ConnectionStrings__Postgres=' .env | cut -d '=' -f2-)

export PATH="/usr/bin:/bin:/usr/local/bin"

cd /srv/agribus || exit 1

docker buildx build -f apps/tomato_backend/Dockerfile.migration -t agribus.migration apps/tomato_backend

docker run --network agribus-network-preprod -e ConnectionStrings__Postgres="$ConnectionStrings__Postgres" agribus.migration:latest
