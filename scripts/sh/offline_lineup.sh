#!/bin/bash

./no-commit/offline_lineup.sh

git add .
git commit -m "Offline Lineup $(date +"%Y-%m-%d_%H-%M-%S")"
git push
