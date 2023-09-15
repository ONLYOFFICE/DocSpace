#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $dir

dir=$(builtin cd $rd/../; pwd)
dockerDir="$dir/build/install/docker"

echo "Root directory:" $dir
echo "Docker files root directory:" $dockerDir

local_ip=$(ipconfig getifaddr en0)

echo "LOCAL IP: $local_ip"

doceditor=${local_ip}:5013
login=${local_ip}:5011
client=${local_ip}:5001
portal_url="http://$local_ip"

echo "SERVICE_DOCEDITOR: $doceditor"
echo "SERVICE_LOGIN: $login"
echo "SERVICE_CLIENT: $client"
echo "APP_URL_PORTAL: $portal_url"

force=false

if [ "$1" = "--force" ]; then
    force=true
fi

echo "FORCE BUILD BASE IMAGES: $force"

# Stop all backend services"
$dir/build/start/stop.backend.docker.sh

echo "Run MySQL"

arch_name="$(uname -m)"

existsnetwork=$(docker network ls | awk '{print $2;}' | { grep -x onlyoffice || true; });

if [[ -z ${existsnetwork} ]]; then
    docker network create --driver bridge onlyoffice
fi

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

echo "Run local dns server"
ROOT_DIR=$dir \
docker compose -f $dockerDir/dnsmasq.yml up -d

echo "Clear publish folder"
rm -rf $dir/publish

echo "Build backend services (to "publish/" folder)"
bash $dir/build/install/common/build-services.sh -pb backend-publish -pc Debug -de "$dockerDir/docker-entrypoint.py"

DOCUMENT_SERVER_IMAGE_NAME=onlyoffice/documentserver-de:latest
INSTALLATION_TYPE=ENTERPRISE

if [ "$1" = "--community" ]; then
    DOCUMENT_SERVER_IMAGE_NAME=onlyoffice/documentserver:latest
    INSTALLATION_TYPE=COMMUNITY
fi

echo "Run migration and services INSTALLATION_TYPE=$INSTALLATION_TYPE"
dotnet_version=dev

exists=$(docker images | egrep "onlyoffice/4testing-docspace-dotnet-runtime" | egrep "$dotnet_version" | awk 'NR>0 {print $1 ":" $2}') 

if [ "${exists}" = "" ] || [ "$force" = true ]; then
    echo "Build dotnet base image from source (apply new dotnet config)"
    docker build -t onlyoffice/4testing-docspace-dotnet-runtime:$dotnet_version  -f ./build/install/docker/Dockerfile.runtime --target dotnetrun .
else 
    echo "SKIP build dotnet base image (already exists)"
fi

node_version=dev

exists=$(docker images | egrep "onlyoffice/4testing-docspace-nodejs-runtime" | egrep "$node_version" | awk 'NR>0 {print $1 ":" $2}') 

if [ "${exists}" = "" ] || [ "$force" = true ]; then
    echo "Build nodejs base image from source"
    docker build -t onlyoffice/4testing-docspace-nodejs-runtime:$node_version  -f ./build/install/docker/Dockerfile.runtime --target noderun .
else 
    echo "SKIP build nodejs base image (already exists)"
fi

proxy_version=dev

exists=$(docker images | egrep "onlyoffice/4testing-docspace-proxy-runtime" | egrep "$proxy_version" | awk 'NR>0 {print $1 ":" $2}') 

if [ "${exists}" = "" ] || [ "$force" = true ]; then
    echo "Build proxy base image from source (apply new nginx config)"
    docker build -t onlyoffice/4testing-docspace-proxy-runtime:$proxy_version  -f ./build/install/docker/Dockerfile.runtime --target router .
else 
    echo "SKIP build proxy base image (already exists)"
fi

echo "Run migration and services"
ENV_EXTENSION="dev" \
INSTALLATION_TYPE=$INSTALLATION_TYPE \
Baseimage_Dotnet_Run="onlyoffice/4testing-docspace-dotnet-runtime:$dotnet_version" \
Baseimage_Nodejs_Run="onlyoffice/4testing-docspace-nodejs-runtime:$node_version" \
Baseimage_Proxy_Run="onlyoffice/4testing-docspace-proxy-runtime:$proxy_version" \
DOCUMENT_SERVER_IMAGE_NAME=$DOCUMENT_SERVER_IMAGE_NAME \
SERVICE_DOCEDITOR=$doceditor \
SERVICE_LOGIN=$login \
SERVICE_CLIENT=$client \
ROOT_DIR=$dir \
BUILD_PATH="/var/www" \
SRC_PATH="$dir/publish/services" \
DATA_DIR="$dir/Data" \
APP_URL_PORTAL=$portal_url \
docker-compose -f $dockerDir/docspace.profiles.yml -f $dockerDir/docspace.overcome.yml --profile migration-runner --profile backend-local up -d

echo ""
echo "APP_URL_PORTAL: $portal_url"
echo "LOCAL IP: $local_ip"
echo "SERVICE_DOCEDITOR: $doceditor"
echo "SERVICE_LOGIN: $login"
echo "SERVICE_CLIENT: $client"
echo "INSTALLATION_TYPE=$INSTALLATION_TYPE"