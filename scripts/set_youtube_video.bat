set WEBSITE_DIR=C:\Github\BaseballWebsite
set LINEUP_GEN_EXE=C:\Github\LineupGen\bin\Release\net9.0\LineupGen.exe

rem publish lineup
%LINEUP_GEN_EXE% "youtube" "%1"

cd "%WEBSITE_DIR%"
git pull
git commit -am "Sync Latest Youtube Video %DATE:~-4%_%DATE:~4,2%_%DATE:~7,2%_%TIME:~0,2%_%TIME:~3,2%_%TIME:~6,2%"
git push

cd "%~dp0"
