#!/bin/bash

WEBSITE_DIR="/home/server/Documents/Github/BaseballWebsite"
TEAM_ROOT_DIR="$WEBSITE_DIR/2025/Mustang/Cubs"
LINEUP_GEN_REPO="/home/server/Documents/Github/LineupGen"
LINEUP_GEN_EXE="$LINEUP_GEN_REPO/bin/Release/net80/LineupGen"


timestamp=$(date +"%Y-%m-%d_%H-%M-%S")

cd "$WEBSITE_DIR"
git pull

# publish lineup
$LINEUP_GEN_EXE -m archive -r "$WEBSITE_DIR" -t "$TEAM_ROOT_DIR"


git add .
git commit -m "Archive Lineup $timestamp"
git push
