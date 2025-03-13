#!/bin/bash

WEBSITE_DIR="media/Storage/Github/BaseballWebsite"
LINEUP_GEN_REPO="media/Storage/Github/LineupGen"
LINEUP_GEN_EXE="$LINEUP_GEN_REPO/bin/Release/net80/LineupGen"
GIT_BIN="/usr/bin/git"

timestamp=$(date +"%Y-%m-%d_%H-%M-%S")

cd "$WEBSITE_DIR"
$GIT_BIN pull

# publish lineup
$LINEUP_GEN_EXE -m offline -r "$WEBSITE_DIR"


if ["$1" == "no-commit"]; then
    echo "Skipping Git Commit..."
    return 0
fi

$GIT_BIN add .
$GIT_BIN commit -m "Offline Lineup $timestamp"
$GIT_BIN push
