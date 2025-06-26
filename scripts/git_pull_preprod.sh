#!/bin/bash

export PATH="/usr/bin:/bin:/usr/local/bin"

cd /srv/agribus || exit 1

git checkout preprod
git pull origin preprod
