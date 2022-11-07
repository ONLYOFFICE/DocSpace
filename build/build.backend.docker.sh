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

docker_file=Dockerfile.dev
core_base_domain="localhost"
build_date=$(date +%Y-%m-%d)
env_extension="dev"

echo "BUILD DATE: $build_date"

local_ip=$(ipconfig getifaddr en0)

echo "LOCAL IP: $local_ip"

doceditor=${local_ip}:5013
login=${local_ip}:5011
client=${local_ip}:5001

echo "SERVICE_DOCEDITOR: $doceditor"
echo "SERVICE_LOGIN: $login"
echo "SERVICE_CLIENT: $client"

# Stop all backend services"
$dir/build/start/stop.backend.docker.sh

echo "Remove all backend containers"
docker rm -f $(docker ps -a | egrep "onlyoffice" | egrep -v "mysql|rabbitmq|redis|elasticsearch|documentserver" | awk 'NR>0 {print $1}')
echo "Remove all backend images"
docker rmi -f $(docker images -a | egrep "onlyoffice" | egrep -v "mysql|rabbitmq|redis|elasticsearch|documentserver" | awk 'NR>0 {print $3}')

echo "Remove all docker images except 'mysql, rabbitmq, redis, elasticsearch, documentserver'"
docker image rm -f $(docker images -a | egrep "onlyoffice" | egrep -v "mysql|rabbitmq|redis|elasticsearch|documentserver" | awk 'NR>0 {print $3}')

echo "Run MySQL"

arch_name="$(uname -m)"

if [ "${arch_name}" = "x86_64" ]; then
    echo "CPU Type: x86_64 -> run db.yml"
    docker compose -f db.yml up -d
elif [ "${arch_name}" = "arm64" ]; then
    echo "CPU Type: arm64 -> run db.yml with arm64v8 image"
    MYSQL_IMAGE=arm64v8/mysql:oracle \
    docker compose -f db.yml up -d
else
    echo "Error: Unknown CPU Type: ${arch_name}."
    exit 1
fi

echo "Run environments (redis, rabbitmq)"
DOCKERFILE=$docker_file \
docker compose -f redis.yml -f rabbitmq.yml up -d

if [ "$1" = "--no_ds" ]; then
    echo "SKIP Document server"
else 
    echo "Run Document server"
    DOCUMENT_SERVER_IMAGE_NAME=onlyoffice/documentserver-de:latest \
    ROOT_DIR=$dir \
    docker compose -f ds.dev.yml up -d
fi

echo "Build all backend services"
DOCKERFILE=$docker_file \
RELEASE_DATE=$build_date \
GIT_BRANCH=$branch \
SERVICE_DOCEDITOR=$doceditor \
SERVICE_LOGIN=$login \
SERVICE_CLIENT=$client \
APP_CORE_BASE_DOMAIN=$core_base_domain \
ENV_EXTENSION=$env_extension \
docker compose -f build.dev.yml build --build-arg GIT_BRANCH=$branch --build-arg RELEASE_DATE=$build_date

echo "Run DB migration"
DOCKERFILE=$docker_file \
docker compose -f migration-runner.yml up -d

# Start all backend services"
$dir/build/start/start.backend.docker.sh