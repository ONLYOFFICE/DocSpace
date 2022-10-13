#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $dir

dir=$(builtin cd $rd/../; pwd)

echo "Root directory:" $dir

cd $dir

branch=$(git branch | sed -n -e 's/^\* \(.*\)/\1/p')

echo "GIT_BRANCH:" $branch

cd $dir/build/install/docker/

docker_dir="$( pwd )"

echo "Docker directory:" $docker_dir

build_date=$(date +%Y-%m-%d)

echo "BUILD DATE: $build_date"

local_ip=$(ipconfig getifaddr en0)

echo "LOCAL IP: $local_ip"

doceditor=${local_ip}:5013
login=${local_ip}:5011
client=${local_ip}:5001

echo "SERVICE_DOCEDITOR: $doceditor"
echo "SERVICE_LOGIN: $login"
echo "SERVICE_CLIENT: $client"

arch_name="$(uname -m)"
 
echo "Run MySQL"

if [ "${arch_name}" = "x86_64" ]; then
    echo "CPU Type: x86_64 -> run db.yml"
    docker compose -f db.yml up -d
elif [ "${arch_name}" = "arm64" ]; then
    echo "CPU Type: arm64 -> run ddb.arm.yml"
    MYSQL_IMAGE=arm64v8/mysql:oracle \
    docker compose -f db.yml up -d
else
    echo "Error: Unknown CPU Type: ${arch_name}."
    exit 1
fi

echo "Run environments (redis, rabbitmq, document-server)"
DOCKERFILE=Dockerfile.dev \
docker compose -f redis.yml -f rabbitmq.yml -f ds.yml up -d

echo "Stop all backend services"
DOCKERFILE=Dockerfile.dev \
docker compose -f docspace.dev.yml down

echo "Build all backend services"
DOCKERFILE=Dockerfile.dev \
RELEASE_DATE=$build_date \
GIT_BRANCH=$branch \
SERVICE_DOCEDITOR=$doceditor \
SERVICE_LOGIN=$login \
SERVICE_CLIENT=$client \
docker compose -f build.dev.yml build

echo "Run DB migration"
DOCKERFILE=Dockerfile.dev \
docker compose -f migration-runner.yml

echo "Start all backend services"
DOCKERFILE=Dockerfile.dev \
ROOT_DIR=$dir \
RELEASE_DATE=$build_date \
GIT_BRANCH=$branch \
SERVICE_DOCEDITOR=$doceditor \
SERVICE_LOGIN=$login \
SERVICE_CLIENT=$client \
APP_URL_PORTAL="http://$local_ip:8092" \
docker compose -f docspace.dev.yml up -d