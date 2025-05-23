#!/bin/bash



WEBSITE_DIR="/media/Storage/Github/BaseballWebsite"
TEAM_ROOT_DIR="$WEBSITE_DIR/2025/Mustang/Cubs"
LINEUP_GEN_REPO="/media/Storage/Github/LineupGen"
LINEUPS_REPO="/media/Storage/Github/BaseballLineups"
LINEUP_GEN_EXE="$LINEUP_GEN_REPO/bin/Release/net80/LineupGen"


cd "$WEBSITE_DIR"
git pull

# publish lineup
$LINEUP_GEN_EXE -m publish -r "$WEBSITE_DIR" -t "$TEAM_ROOT_DIR" -d "$LINEUPS_REPO"


