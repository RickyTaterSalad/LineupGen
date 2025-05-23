#!/bin/bash

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

$SCRIPT_DIR/no-commit/publish_lineup.sh


timestamp=$(date +"%Y-%m-%d_%H-%M-%S")

TEAM_ROOT_DIR="$WEBSITE_DIR/2025/Mustang/Cubs"
LINEUP_GEN_REPO="/media/Storage/Github/LineupGen"
LINEUPS_REPO="/media/Storage/Github/BaseballLineups"
LINEUP_GEN_EXE="$LINEUP_GEN_REPO/bin/Release/net80/LineupGen"

WEBSITE_DIR="/media/Storage/Github/BaseballWebsite"
cd "$WEBSITE_DIR"

git add .
git commit -m "Publish Lineup $timestamp"
git push

cd "$LINEUPS_REPO"
git pull
git add .
git commit -m "Publish Lineup $timestamp"
git push

