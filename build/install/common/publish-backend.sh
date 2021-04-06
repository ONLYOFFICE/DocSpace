#!/bin/bash

SRC_PATH="/AppServer"
BUILD_PATH="/publish"
RID_ID="linux-x64"
SELF_CONTAINED="false"
ARGS=""


while [ "$1" != "" ]; do
    case $1 in

        -sp | --srcpath )
        	if [ "$2" != "" ]; then
    				SRC_PATH=$2
            BUILD_PATH=${SRC_PATH}/publish
		    		shift
		    	fi
		;;
        -bp | --buildpath )
          if [ "$2" != "" ]; then
            BUILD_PATH=$2
            shift
          fi
    ;;
        -ri | --runtime )
          if [ "$2" != "" ]; then
            RID_ID=$2
            shift
          fi
    ;;
        -sc | --self-contained )
          if [ "$2" != "" ]; then
            SELF_CONTAINED=$2
            shift
          fi
    ;;
        -ar | --arguments )
          if [ "$2" != "" ]; then
            ARGS=$2
            shift
          fi
    ;;
        -? | -h | --help )
            echo " Usage: bash publish-backend.sh [PARAMETER] [[PARAMETER], ...]"
            echo "    Parameters:"
            echo "      -sp, --srcpath             path to AppServer root directory (by default=/AppServer)"
            echo "      -bp, --buildpath           path where generated output is placed (by default=/publish)"
            echo "      -ri, --runtime             RID ids for .NET runtime publish (by default=linux-x64)"
            echo "      -sc, --self-contained      publish the .NET runtime with your application (by default=false)"
            echo "      -ar, --arguments           additional arguments publish the .NET runtime with your application"
            echo "      -?, -h, --help             this help"
            echo "  Examples"
            echo "  bash publish-backend.sh -sp /app/AppServer"
            exit 0
    ;;

		* )
			echo "Unknown parameter $1" 1>&2
			exit 1
		;;
    esac
  shift
done

# Array of names server in directory products
servers_products_name_backend=(
    ASC.CRM ASC.Files ASC.People ASC.Projects
)

# Publish server backend products
for i in ${!servers_products_name_backend[@]}; do
  echo "== Publish ${servers_products_name_backend[$i]}.csproj project =="
  SERVICE_DIR="$(dirname "$(find ${SRC_PATH} -type f -name "${servers_products_name_backend[$i]}".csproj)")"
  cd ${SERVICE_DIR}
  dotnet publish -c Release -r ${RID_ID} --self-contained ${SELF_CONTAINED} ${ARGS} -o ${BUILD_PATH}/products/${servers_products_name_backend[$i]}/server/
done

# Array of names backend services
services_name_backend=(
    ASC.ApiSystem ASC.Data.Backup ASC.Data.Storage.Encryption ASC.Files.Service ASC.Data.Storage.Migration ASC.Notify ASC.Socket.IO.Svc ASC.Studio.Notify ASC.TelegramService ASC.Thumbnails.Svc ASC.UrlShortener.Svc ASC.Web.Api ASC.Web.Studio
)

# Publish backend services
for i in ${!services_name_backend[@]}; do
  echo "== Publish ${services_name_backend[$i]}.csproj project =="
  SERVICE_DIR="$(dirname "$(find ${SRC_PATH} -type f -name "${services_name_backend[$i]}".csproj)")"
  cd ${SERVICE_DIR}
  dotnet publish -c Release -r ${RID_ID} --self-contained ${SELF_CONTAINED} ${ARGS} -o ${BUILD_PATH}/services/${services_name_backend[$i]}/service/
done

# Array of names backend services in directory common (Nodejs)  
services_name_frontend=(
    ASC.Thumbnails ASC.UrlShortener ASC.Socket.IO
)

# Publish backend services (Nodejs) 
for i in ${!services_name_frontend[@]}; do
  echo "== Publish ${services_name_frontend[$i]} project =="
  SERVICE_DIR="$(find ${SRC_PATH} -type d -name ${services_name_frontend[$i]})"
  cd ${SERVICE_DIR}
  mkdir -p ${BUILD_PATH}/services/${services_name_frontend[$i]}/service/ && cp -arfv ./* ${BUILD_PATH}/services/${services_name_frontend[$i]}/service/
done
