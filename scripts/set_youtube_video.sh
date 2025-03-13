#!/bin/bash

WEBSITE_DIR="/home/server/Documents/Github/BaseballWebsite"
TEAM_ROOT_DIR="$WEBSITE_DIR/2025/Mustang/Cubs"
LINEUP_GEN_REPO="/home/server/Documents/Github/LineupGen"
LINEUP_GEN_EXE="$LINEUP_GEN_REPO/bin/Release/net80/LineupGen"


timestamp=$(date +"%Y-%m-%d_%H-%M-%S")

cd "$WEBSITE_DIR"
git pull

$LINEUP_GEN_EXE -m youtube -t "$TEAM_ROOT_DIR" -u "$1"


git add .
git commit -m "Set YouTube $timestamp"
git push
