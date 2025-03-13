#!/bin/bash

if [-z "$1"]; then
    return 1
fi

WEBSITE_DIR="media/Storage/Github/BaseballWebsite"
TEAM_ROOT_DIR="$WEBSITE_DIR/2025/Mustang/Cubs"
LINEUP_GEN_REPO="media/Storage/Github/LineupGen"
LINEUP_GEN_EXE="$LINEUP_GEN_REPO/bin/Release/net80/LineupGen"
GIT_BIN="/usr/bin/git"


timestamp=$(date +"%Y-%m-%d_%H-%M-%S")

cd "$WEBSITE_DIR"
$GIT_BIN pull

$LINEUP_GEN_EXE -m youtube -t "$TEAM_ROOT_DIR" -u "$1"

if ["$2" == "no-commit"]; then
    echo "Skipping Git Commit..."
    return 0
fi


$GIT_BIN add .
$GIT_BIN commit -m "Set YouTube $timestamp"
$GIT_BIN push
