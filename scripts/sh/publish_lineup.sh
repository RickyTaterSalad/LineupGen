#!/bin/bash

WEBSITE_DIR="media/Storage/Github/BaseballWebsite"
TEAM_ROOT_DIR="$WEBSITE_DIR/2025/Mustang/Cubs"
LINEUP_GEN_REPO="media/Storage/Github/LineupGen"
LINEUPS_REPO="media/Storage/Github/BaseballLineups"
LINEUP_GEN_EXE="$LINEUP_GEN_REPO/bin/Release/net80/LineupGen"
GIT_BIN="/usr/bin/git"


timestamp=$(date +"%Y-%m-%d_%H-%M-%S")
echo $timestamp
cd "$WEBSITE_DIR"
$GIT_BIN pull

# publish lineup
echo publishing
$LINEUP_GEN_EXE -m publish -r "$WEBSITE_DIR" -t "$TEAM_ROOT_DIR" -d "$LINEUPS_REPO"
echo published

if ["$1" == "no-commit"]; then
    echo "Skipping Git Commit..."
    return 0
fi


$GIT_BIN add .
$GIT_BIN commit -m "Publish Lineup $timestamp"
$GIT_BIN push

cd "$LINEUPS_REPO"
$GIT_BIN pull
$GIT_BIN add .
$GIT_BIN commit -m "Publish Lineup $timestamp"
$GIT_BIN push

