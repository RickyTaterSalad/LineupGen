#!/bin/bash


source ./_setupVars.sh

cd "$WEBSITE_DIR"
git pull

# publish lineup
$LINEUP_GEN_EXE -m publish -r "$WEBSITE_DIR" -t "$TEAM_ROOT_DIR" -d "$LINEUPS_REPO"


