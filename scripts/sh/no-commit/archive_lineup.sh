#!/bin/bash

WEBSITE_DIR="/media/Storage/Github/BaseballWebsite"
TEAM_ROOT_DIR="$WEBSITE_DIR/2025/Mustang/Cubs"
LINEUP_GEN_REPO="/media/Storage/Github/LineupGen"
LINEUP_GEN_EXE="$LINEUP_GEN_REPO/bin/Release/net80/LineupGen"

timestamp=$(date +"%Y-%m-%d_%H-%M-%S")

cd "$WEBSITE_DIR"
git pull

# publish lineup
$LINEUP_GEN_EXE -m archive -r "$WEBSITE_DIR" -t "$TEAM_ROOT_DIR"

