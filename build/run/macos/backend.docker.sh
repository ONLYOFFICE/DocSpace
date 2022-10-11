#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $dir

dir=$(builtin cd $rd/../../../; pwd)
echo "Root directory:" $dir

cd $dir/build/install/docker/

docker_dir="$( pwd )"

echo "Docker directory:" $docker_dir

docker compose --env-file .env.dev -f build.dev.yml build
docker compose --env-file .env.dev -f migration-runner.yml -f docspace.dev.yml up -d