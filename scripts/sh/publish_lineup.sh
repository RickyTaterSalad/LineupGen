#!/bin/bash

LINEUPS_REPO="/media/Storage/Github/BaseballLineups"

timestamp=$(date +"%Y-%m-%d_%H-%M-%S")

./no-commit/publish_lineup.sh

git add .
git commit -m "Publish Lineup $timestamp"
git push

cd "$LINEUPS_REPO"
git pull
git add .
git commit -m "Publish Lineup $timestamp"
git push

