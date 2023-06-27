#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $dir

dir=$(builtin cd $rd/../; pwd)

echo "Root directory:" $dir

cd $dir

branch=$(git branch --show-current)

echo "GIT_BRANCH:" $branch

branch_exist_remote=$(git ls-remote --heads origin $branch)

if [ -z "$branch_exist_remote" ]; then
    echo "The current branch does not exist in the remote repository. Please push changes."
    exit 1
fi

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

echo "Run MySQL"

arch_name="$(uname -m)"

if [ "${arch_name}" = "x86_64" ]; then
    echo "CPU Type: x86_64 -> run db.yml"
    docker compose -f db.yml up -d
elif [ "${arch_name}" = "arm64" ]; then
    echo "CPU Type: arm64 -> run db.yml with arm64v8 image"
    MYSQL_IMAGE=arm64v8/mysql:8.0.32-oracle \
    docker compose -f db.yml up -d
else
    echo "Error: Unknown CPU Type: ${arch_name}."
    exit 1
fi

# if [ "$1" = "--no_ds" ]; then
#     echo "SKIP Document server"
# else 
#     echo "Run Document server"
#     DOCUMENT_SERVER_IMAGE_NAME=onlyoffice/documentserver-de:latest \
#     ROOT_DIR=$dir \
#     docker compose -f ds.dev.yml up -d
# fi

bash $dir/build/install/common/build-services.sh -pb backend-publish -pc Debug -de "$dir/build/install/docker/docker-entrypoint.py"

cd $dir/build/install/docker/

# echo "Build backend services images"
# DOCKERFILE=$docker_file \
# DOCKER_TAG="v9.9.9" \
# RELEASE_DATE=$build_date \
# GIT_BRANCH=$branch \
# SERVICE_DOCEDITOR=$doceditor \
# SERVICE_LOGIN=$login \
# SERVICE_CLIENT=$client \
# APP_CORE_BASE_DOMAIN=$core_base_domain \
# ROOT_DIR=$dir \
# ENV_EXTENSION=$env_extension \
# docker compose -f build.dev.yml --profile backend-dev build --build-arg GIT_BRANCH=$branch --build-arg RELEASE_DATE=$build_date

echo "Run migration"
Baseimage_Dotnet_Run="sk81biz/baseimage-dotnet-run:v1.0.0" \
BUILD_PATH="/var/www" \
SRC_PATH="$dir/publish/services" \
docker-compose -f docspace.yml -f docspace.overcome.yml --profile migration-runner up -d

echo "Run backend services"
Baseimage_Dotnet_Run="sk81biz/baseimage-dotnet-run:v1.0.0" \
Baseimage_Nodejs_Run="sk81biz/baseimage-nodejs-run:v1.0.0" \
Baseimage_Proxy_Run="sk81biz/baseimage-proxy-run:v1.0.0" \
BUILD_PATH="/var/www" \
SRC_PATH="$dir/publish/services" \
SERVICE_DOCEDITOR=$doceditor \
SERVICE_LOGIN=$login \
SERVICE_CLIENT=$client \
ROOT_DIR=$dir \
DATA_DIR="$dir/Data" \
ENV_EXTENSION=$env_extension \
docker-compose -f docspace.yml -f docspace.overcome.yml --profile backend-local up -d

# Start all backend services"
# $dir/build/start/start.backend.docker.sh