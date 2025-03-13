#!/bin/bash

if [-z "$1"]; then
    return 1
fi

WEBSITE_DIR="/media/Storage/Github/BaseballWebsite"
TEAM_ROOT_DIR="$WEBSITE_DIR/2025/Mustang/Cubs"
LINEUP_GEN_REPO="/media/Storage/Github/LineupGen"
LINEUPS_REPO="/media/Storage/Github/BaseballLineups"
LINEUP_GEN_EXE="$LINEUP_GEN_REPO/bin/Release/net80/LineupGen"


cd "$WEBSITE_DIR"

git pull

$LINEUP_GEN_EXE -m archive -r "$WEBSITE_DIR" -t "$TEAM_ROOT_DIR"
$LINEUP_GEN_EXE -m offline -r "$WEBSITE_DIR"
$LINEUP_GEN_EXE -m youtube -t "$TEAM_ROOT_DIR" -u "$1"

