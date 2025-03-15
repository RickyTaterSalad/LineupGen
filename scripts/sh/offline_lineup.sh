#!/bin/bash

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

$SCRIPT_DIR/no-commit/offline_lineup.sh


WEBSITE_DIR="/media/Storage/Github/BaseballWebsite"
cd "$WEBSITE_DIR"

git add .
git commit -m "Offline Lineup $(date +"%Y-%m-%d_%H-%M-%S")"
git push
