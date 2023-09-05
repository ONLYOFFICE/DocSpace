#!/bin/bash
set -xe

SRC_PATH="/AppServer"
BUILD_PATH="/publish"
SELF_CONTAINED="false"
ARGS=""
PUBLISH_CNF="Release"

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
	    -pc | --publish-configuration )
          if [ "$2" != "" ]; then
            PUBLISH_CNF=$2
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
            echo "      -pc, --publish-configuration dotnet publish configuration Ex. Release/Debug"
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
servers_products_name_backend=()
servers_products_name_backend+=(ASC.Files)
servers_products_name_backend+=(ASC.People)

# Publish server backend products
for i in ${!servers_products_name_backend[@]}; do
  echo "== Publish ${servers_products_name_backend[$i]}.csproj project =="
  SERVICE_DIR="$(dirname "$(find ${SRC_PATH} -type f -name "${servers_products_name_backend[$i]}".csproj)")"
  cd ${SERVICE_DIR}
  dotnet publish -c ${PUBLISH_CNF} --self-contained ${SELF_CONTAINED} ${ARGS} -o ${BUILD_PATH}/products/${servers_products_name_backend[$i]}/server/
done

# Array of names backend services
services_name_backend=()
services_name_backend+=(ASC.Data.Backup)
services_name_backend+=(ASC.Files.Service)
services_name_backend+=(ASC.Notify)
services_name_backend+=(ASC.Studio.Notify)
services_name_backend+=(ASC.Web.Api)
services_name_backend+=(ASC.Web.Studio)
services_name_backend+=(ASC.Data.Backup.BackgroundTasks)
services_name_backend+=(ASC.ClearEvents)
services_name_backend+=(ASC.ApiSystem)
services_name_backend+=(ASC.Web.HealthChecks.UI)

# Publish backend services
for i in ${!services_name_backend[@]}; do
  echo "== Publish ${services_name_backend[$i]}.csproj project =="
  SERVICE_DIR="$(dirname "$(find ${SRC_PATH} -type f -name "${services_name_backend[$i]}".csproj)")"
  cd ${SERVICE_DIR}
  dotnet publish -c ${PUBLISH_CNF} --self-contained ${SELF_CONTAINED} ${ARGS} -o ${BUILD_PATH}/services/${services_name_backend[$i]}/service/
done

# Array of names backend services in directory common (Nodejs)
services_name_backend_nodejs=()  
services_name_backend_nodejs+=(ASC.Socket.IO)
services_name_backend_nodejs+=(ASC.SsoAuth)
services_name_backend_nodejs+=(ASC.TelegramReports)

# Publish backend services (Nodejs) 
for i in ${!services_name_backend_nodejs[@]}; do
  echo "== Publish ${services_name_backend_nodejs[$i]} project =="
  SERVICE_DIR="$(find ${SRC_PATH} -type d -name ${services_name_backend_nodejs[$i]})"
  cd ${SERVICE_DIR}
  mkdir -p ${BUILD_PATH}/services/${services_name_backend_nodejs[$i]}/service/ && cp -arfv ./* ${BUILD_PATH}/services/${services_name_backend_nodejs[$i]}/service/
done
