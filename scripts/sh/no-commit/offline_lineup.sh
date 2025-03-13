#!/bin/bash

source ./_setupVars.sh


cd "$WEBSITE_DIR"
git pull

# publish lineup
$LINEUP_GEN_EXE -m offline -r "$WEBSITE_DIR"
