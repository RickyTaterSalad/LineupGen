#!/bin/bash

./no-commit/archive_lineup.sh

git add .
git commit -m "Archive Lineup $(date +"%Y-%m-%d_%H-%M-%S")"
git push
