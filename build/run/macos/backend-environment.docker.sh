#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $dir

dir=$(builtin cd $rd/../../../; pwd)
echo "Root directory:" $dir

cd $dir/build/install/docker/

docker_dir="$( pwd )"

echo "Docker directory:" $docker_dir

docker compose --env-file .env.dev -f db.arm.yml -f redis.yml -f rabbitmq.yml up -d