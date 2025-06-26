#!/bin/bash

export PATH="/usr/bin:/bin:/usr/local/bin"

set -a
source ../.env
set +a

cd $PROJECT_PATH || exit 1

git checkout preprod
git pull origin preprod
