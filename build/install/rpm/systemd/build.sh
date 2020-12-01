#!/bin/bash

PRODUCT="onlyoffice"
BASE_DIR="/app/${PRODUCT}"
PATH_TO_CONF="${BASE_DIR}/config"
LOG_DIR="/var/log/${PRODUCT}"
DOTNET_RUN="/usr/bin/dotnet"
APP_URLS="http://0.0.0.0"
ENVIRONMENT=" --ENVIRONMENT=test"

service_name=(
	mysqld
	kafka
	kafka_zookeeper
	api
	api_system
	urlshortener
	thumbnails
	studio_notify
	notify 
	people
	files
	files_service
	studio
	backup
	nginx)

SERVICE_PORT="" 
SERVICE_NAME=""
WORK_DIR=""
EXEC_FILE=""
CORE=""

reassign_values (){
  case $1 in
	api )	
		SERVICE_NAME="$1"
		SERVICE_PORT="5000"
		WORK_DIR="/var/www/studio/api/"
		EXEC_FILE="ASC.Web.Api.dll"
	;;
	api_system )	
		SERVICE_NAME="$1"
		SERVICE_PORT="5010"
		WORK_DIR="/var/www/services/apisystem/"
		EXEC_FILE="ASC.ApiSystem.dll"
	;;
	urlshortener )
		SERVICE_NAME="$1"
		SERVICE_PORT="5015"
		WORK_DIR="/services/urlshortener/service/"
		EXEC_FILE="ASC.UrlShortener.Svc.dll"
	;;
	thumbnails )	
		SERVICE_NAME="$1"
		SERVICE_PORT="5016"
		WORK_DIR="/services/thumb/service/"
		EXEC_FILE="ASC.Thumbnails.Svc.dll"
	;;
	studio_notify )
		SERVICE_NAME="$1"
		SERVICE_PORT="5006"
		WORK_DIR="/var/www/services/studio.notify/"
		EXEC_FILE="ASC.Studio.Notify.dll"
		CORE=" --core:products:folder=/var/www/products --core:products:subfolder=server "
	;;
	notify )
		SERVICE_NAME="$1"
		SERVICE_PORT="5005"
		WORK_DIR="/var/www/services/notify/"
		EXEC_FILE="ASC.Notify.dll"
		CORE=" --core:products:folder=/var/www/products --core:products:subfolder=server "
	;;
	people )
		SERVICE_NAME="$1"
		SERVICE_PORT="5004"
		WORK_DIR="/var/www/products/ASC.People/server/"
		EXEC_FILE="ASC.People.dll"
	;;
	files )
		SERVICE_NAME="$1"
		SERVICE_PORT="5007"
		WORK_DIR="/var/www/products/ASC.Files/server/"
		EXEC_FILE="ASC.Files.dll"
	;;
	files_service )
		SERVICE_NAME="$1"
		SERVICE_PORT="5009"
		WORK_DIR="/var/www/products/ASC.Files/service/"
		EXEC_FILE="ASC.Files.Service.dll"
		CORE=" --core:products:folder=/var/www/products --core:products:subfolder=server"
	;;
	studio )
		SERVICE_NAME="$1"
		SERVICE_PORT="5003"
		WORK_DIR="/var/www/studio/server/"
		EXEC_FILE="ASC.Web.Studio.dll"
	;;
	backup )
		SERVICE_NAME="$1"
		SERVICE_PORT="5012"
		WORK_DIR="/var/www/services/backup/"
		EXEC_FILE="ASC.Data.Backup.dll"
		CORE=" --core:products:folder=/var/www/products --core:products:subfolder=server"
	;;
  esac
  
  EXEC_START="${DOTNET_RUN} ${WORK_DIR}${EXEC_FILE} --urls=${APP_URLS}:${SERVICE_PORT} --pathToConf=${PATH_TO_CONF} --\$STORAGE_ROOT=/app/onlyoffice/data/ --log:dir=${LOG_DIR} --log:name=${SERVICE_NAME}${CORE}${ENVIRONMENT}"
  
  case $1 in
	nginx )
		SERVICE_NAME="$1"
		EXEC_START='/usr/sbin/nginx -g "daemon off;"'
	;;
	mysqld )	
		EXEC_START="/usr/bin/pidproxy /var/mysqld/mysqld.pid /usr/bin/mysqld_safe"
	;;
	kafka )
		SERVICE_NAME="$1"
		WORK_DIR="/root/kafka_2.12-2.5.0/"
		EXEC_START="/root/kafka_2.12-2.5.0/bin/kafka-server-start.sh /root/kafka_2.12-2.5.0/config/server.properties"
	;;
	kafka_zookeeper )
		SERVICE_NAME="$1"
		WORK_DIR="/root/kafka_2.12-2.5.0/"
		EXEC_START="/root/kafka_2.12-2.5.0/bin/zookeeper-server-start.sh /root/kafka_2.12-2.5.0/config/zookeeper.properties"
	;;
  
  esac
}

write_to_file () {
  sed -i -e 's#${SERVICE_NAME}#'$SERVICE_NAME'#g' -e 's#${WORK_DIR}#'$WORK_DIR'#g' -e \
  "s#\${EXEC_START}#$EXEC_START#g" modules/appserver-${service_name[$i]}.service
}

mkdir -p modules
touch modules/modules

for i in ${!service_name[@]}; do
  echo ${service_name[$i]} >> modules/modules
  cp service ./modules/appserver-${service_name[$i]}.service
  reassign_values "${service_name[$i]}"
  write_to_file $i
done
