#!/bin/bash

docker buildx build -f apps/tomato_backend/Dockerfile.migration -t agribus.migration apps/tomato_backend

docker run --network agribus-network -e ConnectionStrings__Postgres="$ConnectionStrings__Postgres" agribus.migration:latest
