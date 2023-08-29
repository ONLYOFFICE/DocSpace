#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $dir

dir=$(builtin cd $rd/../../; pwd)

echo "Root directory:" $dir

cd $dir/build/install/docker/

docker_dir="$( pwd )"

echo "Docker directory:" $docker_dir

local_ip=$(ipconfig getifaddr en0)

[ -z "$local_ip" ] && local_ip=192.168.0.36

echo "LOCAL IP: $local_ip"

doceditor=${local_ip}:5013
login=${local_ip}:5011
client=${local_ip}:5001

echo "SERVICE_DOCEDITOR: $doceditor"
echo "SERVICE_LOGIN: $login"
echo "SERVICE_CLIENT: $client"

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
DOCUMENT_SERVER_IMAGE_NAME=onlyoffice/documentserver:latest \
docker-compose -f docspace.profiles.yml -f docspace.overcome.yml --profile backend-local start