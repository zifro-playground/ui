#!/bin/bash

# Set error flags
set -o nounset
set -o errexit

ACCOUNT=${1:-"applejag"}

echo ">>> Building $ACCOUNT/ui-upm docker image"
docker build . -t $ACCOUNT/ui-upm -f ui-upm.Dockerfile
echo "<<< Successfully built $ACCOUNT/ui-upm docker image"
echo

echo ">>> Uploading $ACCOUNT/ui-upm docker image"
docker push $ACCOUNT/ui-upm
echo "<<< Successfully uploaded $ACCOUNT/ui-upm docker image"
echo

echo "Build and push complete"