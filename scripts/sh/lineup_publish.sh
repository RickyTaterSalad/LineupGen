#!/bin/bash

WEBSITE_DIR="/home/server/Documents/Github/BaseballWebsite"
TEAM_ROOT_DIR="$WEBSITE_DIR/2025/Mustang/Cubs"
LINEUP_GEN_REPO="/home/server/Documents/Github/LineupGen"
LINEUPS_REPO="/home/server/Documents/Github/BaseballLineups"
LINEUP_GEN_EXE="$LINEUP_GEN_REPO/bin/Release/net80/LineupGen"


timestamp=$(date +"%Y-%m-%d_%H-%M-%S")

cd "$WEBSITE_DIR"
git pull

# publish lineup
$LINEUP_GEN_EXE -m publish -r "$WEBSITE_DIR" -t "$TEAM_ROOT_DIR" -d "$LINEUPS_REPO"


git add .
git commit -m "Publish Lineup $timestamp"
git push

cd "$LINEUPS_REPO"
git pull
git add .
git commit -m "Publish Lineup $timestamp"
git push

