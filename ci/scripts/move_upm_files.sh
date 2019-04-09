#!/bin/bash

# Set error flags
set -o nounset
set -o errexit
set -o pipefail

PROJECT=${1?Project (input) folder required.}
upm=${2?UPM (output) folder required.}

# Remove old files in upm/se.zifro.ui
echo ">>> Remove old files"
rm -rfv $upm/se.zifro.ui/*
echo

# Copy over files
echo ">>> Move Playground UI assets"
cp -rv $PROJECT/Assets/Zifro\ Playground\ UI/* $upm/se.zifro.ui/
echo
