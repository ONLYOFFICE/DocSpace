#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $dir

dir=$(builtin cd $rd/../; pwd)

echo "Root directory:" $dir

cd $dir

cd $dir/build/install/docker/

docker_dir="$( pwd )"

echo "Docker directory:" $docker_dir

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

rm -rf $dir/publish

echo "Build backend services (to "publish/" folder)"
bash $dir/build/install/common/build-services.sh -pb backend-publish -pc Debug -de "$dir/build/install/docker/docker-entrypoint.py"

cd $dir/build/install/docker/

echo "Run migration"
Baseimage_Dotnet_Run="onlyoffice/4testing-docspace-dotnet-runtime:v1.0.0" \
Baseimage_Nodejs_Run="onlyoffice/4testing-docspace-nodejs-runtime:v1.0.0" \
Baseimage_Proxy_Run="onlyoffice/4testing-docspace-proxy-runtime:v1.0.0" \
SERVICE_CLIENT=$client \
BUILD_PATH="/var/www" \
SRC_PATH="$dir/publish/services" \
ROOT_DIR=$dir \
DATA_DIR="$dir/Data" \
docker-compose -f docspace.profiles.yml -f docspace.overcome.yml --profile migration-runner up -d

echo "Run backend services"
Baseimage_Dotnet_Run="onlyoffice/4testing-docspace-dotnet-runtime:v1.0.0" \
Baseimage_Nodejs_Run="onlyoffice/4testing-docspace-nodejs-runtime:v1.0.0" \
Baseimage_Proxy_Run="onlyoffice/4testing-docspace-proxy-runtime:v1.0.0" \
BUILD_PATH="/var/www" \
SRC_PATH="$dir/publish/services" \
SERVICE_DOCEDITOR=$doceditor \
SERVICE_LOGIN=$login \
SERVICE_CLIENT=$client \
ROOT_DIR=$dir \
DATA_DIR="$dir/Data" \
ENV_EXTENSION="dev" \
DOCUMENT_SERVER_IMAGE_NAME=onlyoffice/documentserver-de:latest \
docker-compose -f docspace.profiles.yml -f docspace.overcome.yml --profile backend-local up -d