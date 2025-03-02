@echo off
set SCRIPTS_DIR=C:\Github\LineupGen\scripts
set LINEUP_DIR=C:\Github\BaseballLineups
set WEBSITE_DIR=C:\Github\BaseballWebsite
set LINEUP_GEN_EXE=C:\Github\LineupGen\bin\Release\net9.0\LineupGen.exe

rem pull latest lineups
cd "%LINEUP_DIR%"
git pull

rem generate new lineup
%LINEUP_GEN_EXE% "%LINEUP_DIR%"

%SCRIPTS_DIR%\lineup_publish.bat

set OG_DIR="%cd%"
