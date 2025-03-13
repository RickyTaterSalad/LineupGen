#!/bin/bash

if [-z "$1"]; then
    return 1
fi

./no-commit/youtube_publish.sh $1

git add .
git commit -m "Set YouTube $(date +"%Y-%m-%d_%H-%M-%S")"
git push
