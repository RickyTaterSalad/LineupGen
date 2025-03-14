#!/bin/bash

if [-z "$1"]; then
    return 1
fi

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

$SCRIPT_DIR/no-commit/archive_take_offline_set_youtube_video.sh $1

git add .
git commit -m "archive_take_offline_set_youtube_video $(date +"%Y-%m-%d_%H-%M-%S")"
git push
