#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $dir

dir=$(builtin cd $rd/../; pwd)
dockerDir="$dir/build/install/docker"

echo "Root directory:" $dir
echo "Docker files root directory:" $dockerDir

local_ip=$(ipconfig getifaddr en0)

[ -z "$local_ip" ] && local_ip=192.168.0.36

echo "LOCAL IP: $local_ip"

doceditor=${local_ip}:5013
login=${local_ip}:5011
client=${local_ip}:5001
portal_url="http://$local_ip:8092"

echo "SERVICE_DOCEDITOR: $doceditor"
echo "SERVICE_LOGIN: $login"
echo "SERVICE_CLIENT: $client"
echo "APP_URL_PORTAL: $portal_url"

# Stop all backend services"
$dir/build/start/stop.backend.docker.sh

echo "Run MySQL"

arch_name="$(uname -m)"

if [ "${arch_name}" = "x86_64" ]; then
    echo "CPU Type: x86_64 -> run db.yml"
    docker compose -f $dockerDir/db.yml up -d
elif [ "${arch_name}" = "arm64" ]; then
    echo "CPU Type: arm64 -> run db.yml with arm64v8 image"
    MYSQL_IMAGE=arm64v8/mysql:8.0.32-oracle \
    docker compose -f $dockerDir/db.yml up -d
else
    echo "Error: Unknown CPU Type: ${arch_name}."
    exit 1
fi

echo "Clear publish folder"
rm -rf $dir/publish

echo "Build backend services (to "publish/" folder)"
bash $dir/build/install/common/build-services.sh -pb backend-publish -pc Debug -de "$dockerDir/docker-entrypoint.py"

echo "Run migration and services"
ENV_EXTENSION="dev" \
Baseimage_Dotnet_Run="onlyoffice/4testing-docspace-dotnet-runtime:v1.0.0" \
Baseimage_Nodejs_Run="onlyoffice/4testing-docspace-nodejs-runtime:v1.0.0" \
Baseimage_Proxy_Run="onlyoffice/4testing-docspace-proxy-runtime:v1.0.0" \
DOCUMENT_SERVER_IMAGE_NAME=onlyoffice/documentserver:latest \
SERVICE_DOCEDITOR=$doceditor \
SERVICE_LOGIN=$login \
SERVICE_CLIENT=$client \
ROOT_DIR=$dir \
BUILD_PATH="/var/www" \
SRC_PATH="$dir/publish/services" \
DATA_DIR="$dir/Data" \
APP_URL_PORTAL=$portal_url \
docker-compose -f $dockerDir/docspace.profiles.yml -f $dockerDir/docspace.overcome.yml --profile migration-runner --profile backend-local up -d