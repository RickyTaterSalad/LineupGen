@echo off
LineupGen.exe "C:\Github\BaseballLineups"
LineupGen.exe "publish"
cd "C:\Github\BaseballWebsite"
SET MSG=Insert Lineup %DATE:~-4%_%DATE:~4,2%_%DATE:~7,2%_%TIME:~0,2%_%TIME:~3,2%_%TIME:~6,2%
git checkout main
git pull
git add .
git commit -a -m "Insert Lineup %MSG%"
git push