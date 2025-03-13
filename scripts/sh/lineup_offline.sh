#!/bin/bash

WEBSITE_DIR="/home/server/Documents/Github/BaseballWebsite"
LINEUP_GEN_REPO="/home/server/Documents/Github/LineupGen"
LINEUP_GEN_EXE="$LINEUP_GEN_REPO/bin/Release/net80/LineupGen"


timestamp=$(date +"%Y-%m-%d_%H-%M-%S")

cd "$WEBSITE_DIR"
git pull

# publish lineup
$LINEUP_GEN_EXE -m offline -r "$WEBSITE_DIR"


git add .
git commit -m "Offline Lineup $timestamp"
git push
