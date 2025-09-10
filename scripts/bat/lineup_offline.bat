@echo off

set WEBSITE_DIR=C:/Repos/BaseballWebsite
set TEAM_ROOT_DIR=%WEBSITE_DIR%/2026/10U/Jaxx
set LINEUP_GEN_REPO=C:/Repos/LineupGen
set LINEUP_GEN_EXE=%LINEUP_GEN_REPO%/bin/Release/net80/LineupGen.exe

%LINEUP_GEN_EXE% -m offline -r "%WEBSITE_DIR%"

cd "%WEBSITE_DIR%"
git pull
git add .
git commit -m "Take Lineup Offline %DATE:~-4%_%DATE:~4,2%_%DATE:~7,2%_%TIME:~0,2%_%TIME:~3,2%_%TIME:~6,2%"
