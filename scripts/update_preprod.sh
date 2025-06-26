#!/bin/bash

REPO_DIR="/srv/agribus"

cd "$REPO_DIR" || exit 1

git fetch origin preprod

LOCAL=$(git rev-parse preprod)
REMOTE=$(git rev-parse origin/preprod)

if [ "$LOCAL" != "$REMOTE" ]; then
    git checkout preprod
    git pull origin preprod
    ./deploy_preprod.sh
fi
