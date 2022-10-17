#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $dir

dir=$(builtin cd $rd/../../; pwd)

echo "Root directory:" $dir

cd $dir/build/install/docker/

docker_dir="$( pwd )"

echo "Docker directory:" $docker_dir

echo "Stop all backend services"
DOCKERFILE=Dockerfile.dev \
docker compose -f docspace.dev.yml down