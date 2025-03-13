#!/bin/bash

source ./_setupVars.sh


cd "$WEBSITE_DIR"
git pull

$LINEUP_GEN_EXE -m archive -r "$WEBSITE_DIR" -t "$TEAM_ROOT_DIR"

