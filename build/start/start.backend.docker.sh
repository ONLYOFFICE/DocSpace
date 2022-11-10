#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $dir

dir=$(builtin cd $rd/../../; pwd)

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

docker_file=Dockerfile.dev
env_extension="dev"
core_base_domain="localhost"

echo "Start all backend services (containers)"
DOCKERFILE=$docker_file \
ROOT_DIR=$dir \
RELEASE_DATE=$build_date \
GIT_BRANCH=$branch \
SERVICE_DOCEDITOR=$doceditor \
SERVICE_LOGIN=$login \
SERVICE_CLIENT=$client \
APP_CORE_BASE_DOMAIN=$core_base_domain \
APP_URL_PORTAL="http://$local_ip:8092" \
ENV_EXTENSION=$env_extension \
docker compose -f docspace.dev.yml up -d