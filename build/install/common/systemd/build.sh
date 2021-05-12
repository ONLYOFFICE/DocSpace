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

PRODUCT="onlyoffice/appserver"
BASE_DIR="/etc/${PRODUCT}"
PATH_TO_CONF="${BASE_DIR}"
STORAGE_ROOT="${BASE_DIR}/data"
LOG_DIR="/var/log/${PRODUCT}"
DOTNET_RUN="/usr/bin/dotnet"
APP_URLS="http://0.0.0.0"
ENVIRONMENT=" --ENVIRONMENT=production"

service_name=(
	ASC.Web.Api
	ASC.ApiSystem
	ASC.UrlShortener
	ASC.Thumbnails
	ASC.Socket
	ASC.Studio.Notify
	ASC.Notify 
	ASC.People
	ASC.Files
	ASC.Files.Service
	ASC.Web.Studio
	ASC.Data.Backup
	ASC.Data.Storage.Encryption
	ASC.Data.Storage.Migration
	ASC.Projects
	ASC.TelegramService
	ASC.CRM
	ASC.Calendar
	ASC.Mail
	)

reassign_values (){
  case $1 in
	ASC.Web.Api )
		SERVICE_PORT="5000"
		WORK_DIR="/var/www/appserver/studio/api/"
		EXEC_FILE="ASC.Web.Api.dll"
	;;
	ASC.ApiSystem )
		SERVICE_PORT="5010"
		WORK_DIR="/var/www/appserver/services/ASC.ApiSystem/"
		EXEC_FILE="ASC.ApiSystem.dll"
	;;
	ASC.UrlShortener )
		SERVICE_PORT="9999"
		WORK_DIR="/var/www/appserver/services/ASC.UrlShortener/"
		EXEC_FILE="ASC.UrlShortener.Svc.dll"
	;;
	ASC.Thumbnails )
		SERVICE_PORT="9800"
		WORK_DIR="/var/www/appserver/services/ASC.Thumbnails/"
		EXEC_FILE="ASC.Thumbnails.Svc.dll"
	;;
	ASC.Socket )
		SERVICE_PORT="9899"
		WORK_DIR="/var/www/appserver/services/ASC.Socket.IO/"
		EXEC_FILE="ASC.Socket.IO.Svc.dll"
	;;
	ASC.Studio.Notify )
		SERVICE_PORT="5006"
		WORK_DIR="/var/www/appserver/services/ASC.Studio.Notify/"
		EXEC_FILE="ASC.Studio.Notify.dll"
		CORE=" --core:products:folder=/var/www/appserver/products --core:products:subfolder=server "
	;;
	ASC.Notify )
		SERVICE_PORT="5005"
		WORK_DIR="/var/www/appserver/services/ASC.Notify/"
		EXEC_FILE="ASC.Notify.dll"
		CORE=" --core:products:folder=/var/www/appserver/products --core:products:subfolder=server "
	;;
	ASC.People )
		SERVICE_PORT="5004"
		WORK_DIR="/var/www/appserver/products/ASC.People/server/"
		EXEC_FILE="ASC.People.dll"
	;;
	ASC.Files )
		SERVICE_PORT="5007"
		WORK_DIR="/var/www/appserver/products/ASC.Files/server/"
		EXEC_FILE="ASC.Files.dll"
	;;
	ASC.Files.Service )
		SERVICE_PORT="5009"
		WORK_DIR="/var/www/appserver/products/ASC.Files/service/"
		EXEC_FILE="ASC.Files.Service.dll"
		CORE=" --core:products:folder=/var/www/appserver/products --core:products:subfolder=server"
	;;
	ASC.Web.Studio )
		SERVICE_PORT="5003"
		WORK_DIR="/var/www/appserver/studio/server/"
		EXEC_FILE="ASC.Web.Studio.dll"
	;;
	ASC.Data.Backup )
		SERVICE_PORT="5012"
		WORK_DIR="/var/www/appserver/services/ASC.Data.Backup/"
		EXEC_FILE="ASC.Data.Backup.dll"
		CORE=" --core:products:folder=/var/www/appserver/products --core:products:subfolder=server"
	;;
	ASC.Data.Storage.Migration )
		SERVICE_PORT="5018"
		WORK_DIR="/var/www/appserver/services/ASC.Data.Storage.Migration/"
		EXEC_FILE="ASC.Data.Storage.Migration.dll"
	;;
	ASC.Data.Storage.Encryption )
		SERVICE_PORT="5019"
		WORK_DIR="/var/www/appserver/services/ASC.Data.Storage.Encryption/"
		EXEC_FILE="ASC.Data.Storage.Encryption.dll"
	;;
	ASC.Projects )
		SERVICE_PORT="5020"
		WORK_DIR="/var/www/appserver/products/ASC.Projects/server/"
		EXEC_FILE="ASC.Projects.dll"
	;;
	ASC.TelegramService )
		SERVICE_PORT="51702"
		WORK_DIR="/var/www/appserver/services/ASC.TelegramService/"
		EXEC_FILE="ASC.TelegramService.dll"
	;;
	ASC.CRM )
		SERVICE_PORT="5021"
		WORK_DIR="/var/www/appserver/products/ASC.CRM/server/"
		EXEC_FILE="ASC.CRM.dll"
	;;
	ASC.Calendar )
		SERVICE_PORT="5023"
		WORK_DIR="/var/www/appserver/products/ASC.Calendar/server/"
		EXEC_FILE="ASC.Calendar.dll"
	;;
	ASC.Mail )
		SERVICE_PORT="5022"
		WORK_DIR="/var/www/appserver/products/ASC.Mail/server/"
		EXEC_FILE="ASC.Mail.dll"
	;;
  esac
  SERVICE_NAME="$1"
  EXEC_START="${DOTNET_RUN} ${WORK_DIR}${EXEC_FILE} --urls=${APP_URLS}:${SERVICE_PORT} --pathToConf=${PATH_TO_CONF} --'\$STORAGE_ROOT'=${STORAGE_ROOT} --log:dir=${LOG_DIR} --log:name=${SERVICE_NAME}${CORE}${ENVIRONMENT}"
  CORE=""
}

write_to_file () {
  sed -i -e 's#${SERVICE_NAME}#'$SERVICE_NAME'#g' -e 's#${WORK_DIR}#'$WORK_DIR'#g' -e \
  "s#\${EXEC_START}#$EXEC_START#g" $BUILD_PATH/AppServer-${service_name[$i]}.service
}

mkdir -p $BUILD_PATH

for i in ${!service_name[@]}; do
  cp $BASEDIR/service $BUILD_PATH/AppServer-${service_name[$i]}.service
  reassign_values "${service_name[$i]}"
  write_to_file $i
done
