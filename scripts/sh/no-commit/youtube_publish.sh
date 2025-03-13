#!/bin/bash

if [-z "$1"]; then
    return 1
fi

source ./_setupVars.sh

cd "$WEBSITE_DIR"
git pull

$LINEUP_GEN_EXE -m youtube -t "$TEAM_ROOT_DIR" -u "$1"
