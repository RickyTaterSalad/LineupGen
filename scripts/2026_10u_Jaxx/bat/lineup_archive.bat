@echo off

set WEBSITE_DIR=C:/Repos/BaseballWebsite
set TEAM_ROOT_DIR=%WEBSITE_DIR%/2026/10U/Jaxx
set LINEUP_GEN_REPO=C:/Repos/LineupGen
set LINEUP_GEN_EXE=%LINEUP_GEN_REPO%/bin/Release/net80/LineupGen.exe

%LINEUP_GEN_EXE% -m "archive" -r "%WEBSITE_DIR%" -t "%TEAM_ROOT_DIR%"

rem cd "%WEBSITE_DIR%"
rem git pull
rem git add .
rem git commit -m "Archive Lineup %DATE:~-4%_%DATE:~4,2%_%DATE:~7,2%_%TIME:~0,2%_%TIME:~3,2%_%TIME:~6,2%"


