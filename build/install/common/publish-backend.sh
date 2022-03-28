#!/bin/bash

SRC_PATH="/AppServer"
BUILD_PATH="/publish"
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
# servers_products_name_backend=(ASC.CRM)
servers_products_name_backend+=(ASC.Files)
servers_products_name_backend+=(ASC.People)
# servers_products_name_backend+=(ASC.Projects)
# servers_products_name_backend+=(ASC.Calendar)
# servers_products_name_backend+=(ASC.Mail)

# Publish server backend products
for i in ${!servers_products_name_backend[@]}; do
  echo "== Publish ${servers_products_name_backend[$i]}.csproj project =="
  SERVICE_DIR="$(dirname "$(find ${SRC_PATH} -type f -name "${servers_products_name_backend[$i]}".csproj)")"
  cd ${SERVICE_DIR}
  dotnet publish -c Release --self-contained ${SELF_CONTAINED} ${ARGS} -o ${BUILD_PATH}/products/${servers_products_name_backend[$i]}/server/
done

# Array of names backend services
services_name_backend=(ASC.ApiSystem)
services_name_backend+=(ASC.Data.Backup)
services_name_backend+=(ASC.Data.Storage.Encryption)
services_name_backend+=(ASC.Files.Service)
services_name_backend+=(ASC.Data.Storage.Migration)
services_name_backend+=(ASC.Notify)
services_name_backend+=(ASC.Socket.IO.Svc)
services_name_backend+=(ASC.Studio.Notify)
services_name_backend+=(ASC.TelegramService)
services_name_backend+=(ASC.Thumbnails.Svc)
services_name_backend+=(ASC.UrlShortener.Svc)
services_name_backend+=(ASC.Web.Api)
services_name_backend+=(ASC.Web.Studio)
services_name_backend+=(ASC.SsoAuth.Svc)

# Publish backend services
for i in ${!services_name_backend[@]}; do
  echo "== Publish ${services_name_backend[$i]}.csproj project =="
  SERVICE_DIR="$(dirname "$(find ${SRC_PATH} -type f -name "${services_name_backend[$i]}".csproj)")"
  cd ${SERVICE_DIR}
  dotnet publish -c Release --self-contained ${SELF_CONTAINED} ${ARGS} -o ${BUILD_PATH}/services/${services_name_backend[$i]}/service/
done

# Array of names backend services in directory common (Nodejs)  
services_name_backend_nodejs=(ASC.Thumbnails)
services_name_backend_nodejs+=(ASC.UrlShortener)
services_name_backend_nodejs+=(ASC.Socket.IO)
services_name_backend_nodejs+=(ASC.SsoAuth)

# Publish backend services (Nodejs) 
for i in ${!services_name_backend_nodejs[@]}; do
  echo "== Publish ${services_name_backend_nodejs[$i]} project =="
  SERVICE_DIR="$(find ${SRC_PATH} -type d -name ${services_name_backend_nodejs[$i]})"
  cd ${SERVICE_DIR}
  mkdir -p ${BUILD_PATH}/services/${services_name_backend_nodejs[$i]}/service/ && cp -arfv ./* ${BUILD_PATH}/services/${services_name_backend_nodejs[$i]}/service/
done
