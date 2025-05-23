@echo off
set WEBSITE_DIR=C:\Github\BaseballWebsite
set LINEUP_GEN_EXE=C:\Github\LineupGen\bin\Release\net9.0\LineupGen.exe

%LINEUP_GEN_EXE% "archive"
%LINEUP_GEN_EXE% "offline"
%LINEUP_GEN_EXE% "youtube" "%1"

cd "%WEBSITE_DIR%"
git pull
git add .
git commit -m "archive_take_offline_set_youtube_video %DATE:~-4%_%DATE:~4,2%_%DATE:~7,2%_%TIME:~0,2%_%TIME:~3,2%_%TIME:~6,2%"
git push

cd "%~dp0"
