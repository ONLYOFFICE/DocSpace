#!/bin/bash

BASEDIR="$(cd $(dirname $0) && pwd)"
BUILD_PATH="$BASEDIR/modules"

while [ "$1" != "" ]; do
    case $1 in
	    
        -bp | --buildpath )
        	if [ "$2" != "" ]; then
				    BUILD_PATH=$2
				    shift
			    fi
		;;

        -? | -h | --help )
            echo " Usage: bash build.sh [PARAMETER] [[PARAMETER], ...]"
            echo "    Parameters:"
            echo "      -bp, --buildpath           output path"
            echo "      -?, -h, --help             this help"
            echo "  Examples"
            echo "  bash build.sh -bp /etc/systemd/system/"
            exit 0
    ;;

		* )
			echo "Unknown parameter $1" 1>&2
			exit 1
		;;
    esac
  shift
done

PRODUCT="appserver"
BASE_DIR="/var/www/${PRODUCT}"
PATH_TO_CONF="/etc/onlyoffice/${PRODUCT}"
STORAGE_ROOT="${PATH_TO_CONF}/data"
LOG_DIR="/var/log/onlyoffice/${PRODUCT}"
DOTNET_RUN="/usr/share/dotnet/dotnet"
APP_URLS="http://0.0.0.0"
ENVIRONMENT=" --ENVIRONMENT=production"
CORE=" --core:products:folder=${BASE_DIR}/products --core:products:subfolder=server"

SERVICE_NAME=(
	api
	api-system
	urlshortener
	thumbnails
	socket
	studio-notify
	notify 
	people-server
	files
	files-services
	studio
	backup
	storage-encryption
	storage-migration
	telegram-service
	ssoauth
	)

reassign_values (){
  case $1 in
	api )
		SERVICE_PORT="5000"
		WORK_DIR="${BASE_DIR}/studio/api/"
		EXEC_FILE="ASC.Web.Api.dll"
	;;
	api-system )
		SERVICE_PORT="5010"
		WORK_DIR="${BASE_DIR}/services/ASC.ApiSystem/"
		EXEC_FILE="ASC.ApiSystem.dll"
	;;
	urlshortener )
		SERVICE_PORT="9998"
		WORK_DIR="${BASE_DIR}/services/ASC.UrlShortener.Svc/"
		EXEC_FILE="ASC.UrlShortener.Svc.dll"
	;;
	thumbnails )
		SERVICE_PORT="9799"
		WORK_DIR="${BASE_DIR}/services/ASC.Thumbnails.Svc/"
		EXEC_FILE="ASC.Thumbnails.Svc.dll"
	;;
	socket )
		SERVICE_PORT="9898"
		WORK_DIR="${BASE_DIR}/services/ASC.Socket.IO.Svc/"
		EXEC_FILE="ASC.Socket.IO.Svc.dll"
	;;
	studio-notify )
		SERVICE_PORT="5006"
		WORK_DIR="${BASE_DIR}/services/ASC.Studio.Notify/"
		EXEC_FILE="ASC.Studio.Notify.dll"
	;;
	notify )
		SERVICE_PORT="5005"
		WORK_DIR="${BASE_DIR}/services/ASC.Notify/"
		EXEC_FILE="ASC.Notify.dll"
	;;
	people-server )
		SERVICE_PORT="5004"
		WORK_DIR="${BASE_DIR}/products/ASC.People/server/"
		EXEC_FILE="ASC.People.dll"
	;;
	files )
		SERVICE_PORT="5007"
		WORK_DIR="${BASE_DIR}/products/ASC.Files/server/"
		EXEC_FILE="ASC.Files.dll"
	;;
	files-services )
		SERVICE_PORT="5009"
		WORK_DIR="${BASE_DIR}/products/ASC.Files/service/"
		EXEC_FILE="ASC.Files.Service.dll"
	;;
	studio )
		SERVICE_PORT="5003"
		WORK_DIR="${BASE_DIR}/studio/server/"
		EXEC_FILE="ASC.Web.Studio.dll"
	;;
	backup )
		SERVICE_PORT="5012"
		WORK_DIR="${BASE_DIR}/services/ASC.Data.Backup/"
		EXEC_FILE="ASC.Data.Backup.dll"
	;;
	storage-migration )
		SERVICE_PORT="5018"
		WORK_DIR="${BASE_DIR}/services/ASC.Data.Storage.Migration/"
		EXEC_FILE="ASC.Data.Storage.Migration.dll"
	;;
	storage-encryption )
		SERVICE_PORT="5019"
		WORK_DIR="${BASE_DIR}/services/ASC.Data.Storage.Encryption/"
		EXEC_FILE="ASC.Data.Storage.Encryption.dll"
	;;
	telegram-service )
		SERVICE_PORT="51702"
		WORK_DIR="${BASE_DIR}/services/ASC.TelegramService/"
		EXEC_FILE="ASC.TelegramService.dll"
	;;
	ssoauth )
		SERVICE_PORT="9833"
		WORK_DIR="${BASE_DIR}/services/ASC.SsoAuth.Svc/"
		EXEC_FILE="ASC.SsoAuth.Svc.dll"
	;;
  esac
  SERVICE_NAME="$1"
  EXEC_START="${DOTNET_RUN} ${WORK_DIR}${EXEC_FILE} --urls=${APP_URLS}:${SERVICE_PORT} --pathToConf=${PATH_TO_CONF} \
  --'\$STORAGE_ROOT'=${STORAGE_ROOT} --log:dir=${LOG_DIR} --log:name=${SERVICE_NAME}${CORE}${ENVIRONMENT}"
}

write_to_file () {
  sed -i -e 's#${SERVICE_NAME}#'$SERVICE_NAME'#g' -e 's#${WORK_DIR}#'$WORK_DIR'#g' -e \
  "s#\${EXEC_START}#$EXEC_START#g" $BUILD_PATH/${PRODUCT}-${SERVICE_NAME[$i]}.service
}

mkdir -p $BUILD_PATH

for i in ${!SERVICE_NAME[@]}; do
  cp $BASEDIR/service $BUILD_PATH/${PRODUCT}-${SERVICE_NAME[$i]}.service
  reassign_values "${SERVICE_NAME[$i]}"
  write_to_file $i
done
