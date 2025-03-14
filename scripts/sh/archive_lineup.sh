#!/bin/bash

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

$SCRIPT_DIR/no-commit/archive_lineup.sh

git add .
git commit -m "Archive Lineup $(date +"%Y-%m-%d_%H-%M-%S")"
git push
