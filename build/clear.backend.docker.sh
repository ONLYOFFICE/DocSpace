#!/bin/bash

Containers=$(docker ps -a | egrep "onlyoffice" | awk 'NR>0 {print $1}')
RunDir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
RootDir=$(builtin cd $RunDir/../; pwd)
DockerDir="${RootDir}/build/install/docker"

echo "Clean up containers, volumes or networks"

if [[ $Containers != "" ]]
then
    echo "Remove all backend containers"

    DOCUMENT_SERVER_IMAGE_NAME=onlyoffice/documentserver:latest \
    Baseimage_Dotnet_Run="onlyoffice/4testing-docspace-dotnet-runtime:v1.0.0" \
    Baseimage_Nodejs_Run="onlyoffice/4testing-docspace-nodejs-runtime:v1.0.0" \
    Baseimage_Proxy_Run="onlyoffice/4testing-docspace-proxy-runtime:v1.0.0" \
    SERVICE_CLIENT="localhost:5001" \
    BUILD_PATH="/var/www" \
    SRC_PATH="${RootDir}/publish/services" \
    ROOT_DIR=$RootDir \
    DATA_DIR="${RootDir}/Data" \
    docker-compose -f "${DockerDir}/docspace.profiles.yml" -f "${DockerDir}/docspace.overcome.yml" --profile migration-runner --profile backend-local down --volumes

    echo "Remove docker contatiners 'mysql'"
    docker compose -f "${DockerDir}/db.yml" down --volumes

    echo "Remove docker volumes"
    docker volume prune -f -a

    echo "Remove unused networks."
    docker network prune -f
else
    echo "No containers, images, volumes or networks to clean up"
fi