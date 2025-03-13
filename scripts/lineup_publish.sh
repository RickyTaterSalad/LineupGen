#!/bin/bash
WEBSITE_DIR="/home/server/Documents/Github/BaseballWebsite"
TEAM_ROOT_DIR="%WEBSITE_DIR%/2025/Mustang/Cubs"
LINEUP_GEN_REPO="/home/server/Documents/Github/LineupGen"
LINEUPS_REPO="/home/server/Documents/Github/BaseballLineups"
LINEUP_GEN_EXE="%LINEUP_GEN_REPO%/bin/Release/net80/LineupGen"

cd "$WEBSITE_DIR"
git pull

# publish lineup
%LINEUP_GEN_EXE% -m publish -t "$TEAM_ROOT_DIR" -d "$LINEUPS_REPO"


#git add .
#git commit -m "Publish Lineup %DATE:~-4%_%DATE:~4,2%_%DATE:~7,2%_%TIME:~0,2%_%TIME:~3,2%_%TIME:~6,2%"
#git push
