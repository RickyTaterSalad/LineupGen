#!/bin/bash



./no-commit/publish_lineup.sh


timestamp=$(date +"%Y-%m-%d_%H-%M-%S")
source ./_setupVars.sh

git add .
git commit -m "Publish Lineup $timestamp"
git push

cd "$LINEUPS_REPO"
git pull
git add .
git commit -m "Publish Lineup $timestamp"
git push

