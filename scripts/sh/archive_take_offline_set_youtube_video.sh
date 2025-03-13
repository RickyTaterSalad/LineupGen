#!/bin/bash

if [-z "$1"]; then
    return 1
fi

./no-commit/archive_take_offline_set_youtube_video.sh

git add .
git commit -m "archive_take_offline_set_youtube_video $(date +"%Y-%m-%d_%H-%M-%S")"
git push
