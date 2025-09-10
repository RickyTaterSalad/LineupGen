@echo off
set WEBSITE_DIR=C:\Repos\BaseballWebsite
set LINEUP_GEN_EXE=C:\Github\LineupGen\bin\Release\net9.0\LineupGen.exe

rem offline lineup
%LINEUP_GEN_EXE% "offline"

cd "%WEBSITE_DIR%"
git pull
git add .
git commit -m "Take Lineup Offline %DATE:~-4%_%DATE:~4,2%_%DATE:~7,2%_%TIME:~0,2%_%TIME:~3,2%_%TIME:~6,2%"
git push

cd "%~dp0"