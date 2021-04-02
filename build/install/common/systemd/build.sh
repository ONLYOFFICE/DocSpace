#!/bin/bash

BASEDIR="$(cd $(dirname $0) && pwd)"
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
	ASC.UrlShortener.Svc
	ASC.Thumbnails.Svc
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
		WORK_DIR="/var/www/appserver/services/apisystem/"
		EXEC_FILE="ASC.ApiSystem.dll"
	;;
	ASC.UrlShortener.Svc )
		SERVICE_PORT="5015"
		WORK_DIR="/var/www/appserver/services/urlshortener/service/"
		EXEC_FILE="ASC.UrlShortener.Svc.dll"
	;;
	ASC.Thumbnails.Svc )
		SERVICE_PORT="5016"
		WORK_DIR="/var/www/appserver/services/thumb/service/"
		EXEC_FILE="ASC.Thumbnails.Svc.dll"
	;;
	ASC.Socket )
		SERVICE_PORT="9999"
		WORK_DIR="/var/www/appserver/services/socket/service/"
		EXEC_FILE="ASC.Socket.IO.Svc.dll"
	;;
	ASC.Studio.Notify )
		SERVICE_PORT="5006"
		WORK_DIR="/var/www/appserver/services/studio.notify/"
		EXEC_FILE="ASC.Studio.Notify.dll"
		CORE=" --core:products:folder=/var/www/appserver/products --core:products:subfolder=server "
	;;
	ASC.Notify )
		SERVICE_PORT="5005"
		WORK_DIR="/var/www/appserver/services/notify/"
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
		WORK_DIR="/var/www/appserver/services/backup/"
		EXEC_FILE="ASC.Data.Backup.dll"
		CORE=" --core:products:folder=/var/www/appserver/products --core:products:subfolder=server"
	;;
	ASC.Data.Storage.Migration )
		SERVICE_PORT="5018"
		WORK_DIR="/var/www/appserver/services/storage.migration/"
		EXEC_FILE="ASC.Data.Storage.Migration.dll"
	;;
	ASC.Data.Storage.Encryption )
		SERVICE_PORT="5019"
		WORK_DIR="/var/www/appserver/services/storage.encryption/"
		EXEC_FILE="ASC.Data.Storage.Encryption.dll"
	;;
	ASC.Projects )
		SERVICE_PORT="5015"
		WORK_DIR="/var/www/appserver/products/ASC.Projects/server/"
		EXEC_FILE="ASC.Projects.dll"
	;;
	ASC.TelegramService )
		SERVICE_PORT="51702"
		WORK_DIR="/var/www/appserver/services/telegram/service/"
		EXEC_FILE="ASC.TelegramService.dll"
	;;
	ASC.CRM )
		SERVICE_PORT="5014"
		WORK_DIR="/var/www/appserver/products/ASC.CRM/server/"
		EXEC_FILE="ASC.CRM.dll"
	;;
  esac
  SERVICE_NAME="$1"
  EXEC_START="${DOTNET_RUN} ${WORK_DIR}${EXEC_FILE} --urls=${APP_URLS}:${SERVICE_PORT} --pathToConf=${PATH_TO_CONF} --'\$STORAGE_ROOT'=${STORAGE_ROOT} --log:dir=${LOG_DIR} --log:name=${SERVICE_NAME}${CORE}${ENVIRONMENT}"
  CORE=""
}

write_to_file () {
  sed -i -e 's#${SERVICE_NAME}#'$SERVICE_NAME'#g' -e 's#${WORK_DIR}#'$WORK_DIR'#g' -e \
  "s#\${EXEC_START}#$EXEC_START#g" $BASEDIR/modules/AppServer-${service_name[$i]}.service
}

mkdir -p $BASEDIR/modules

for i in ${!service_name[@]}; do
  cp $BASEDIR/service $BASEDIR/modules/AppServer-${service_name[$i]}.service
  reassign_values "${service_name[$i]}"
  write_to_file $i
done
