#!/bin/bash

PRODUCT="onlyoffice"
BASE_DIR="/app/${PRODUCT}"
PARAMETERS=""

BUILD_URLS=${APP_URLS:-"http://0.0.0.0:${SERVICE_PORT:-5050}"}
PATH_TO_CONF="${BASE_DIR}/config"
LOG_DIR="/var/log/onlyoffice"

BUILD_MYSQL_HOST=${MYSQL_HOST:-"mysql"}
BUILD_MYSQL_DATABASE=${MYSQL_DATABASE:-${PRODUCT}}
BUILD_MYSQL_USER=${MYSQL_USER:-"${PRODUCT}_user"}
BUILD_MYSQL_PASSWORD=${MYSQL_PASSWORD:-"${PRODUCT}_pass"}

BUILD_KAFKA_HOST=${KAFKA_HOST:-"kafka"}

BUILD_APP_DOTNET_ENV=${APP_DOTNET_ENV:-"test"}
BUILD_APP_CORE_BASE_DOMAIN=${APP_CORE_BASE_DOMAIN:-"localhost"}
BUILD_APP_CORE_MACHINEKEY=${APP_CORE_MACHINEKEY:-""}
BUILD_APP_URL_PUBLIC=${APP_URL_PUBLIC:-"/ds-vpath/"}
BUILD_APP_URL_INTERNAL=${APP_URL_INTERNAL:-"http://onlyoffice-document-server/"}
BUILD_APP_URL_PORTAL=${APP_URL_PORTAL:-"http://onlyoffice-community-server"}
BUILD_APP_URL_CONVERTER=${APP_URL_CONVERTER:-"/ds-vpath/ConvertService.ashx"}
BUILD_APP_VIEWED_MEDIA=${APP_VIEWED_MEDIA:-'".aac",".flac",".m4a",".mp3",".oga",".ogg",".wav",".f4v",".m4v",".mov",".mp4",".ogv",".webm",".avi"'}
BUILD_APP_FFMPEG_EXTS=${APP_FFMPEG_EXTS:-'"avi", "mpeg", "mpg", "wmv"'}
BUILD_APP_JWT_SECRET=${DOCUMENT_SERVER_JWT_SECRET:-""}
BUILD_APP_SECRET_HEADER=${APP_SECRET_HEADER:-"AuthorizationJwt"}
BUILD_STORAGE_ROOT=${APP_STORAGE_ROOT:-"/app/onlyoffice/data/"}

if [ -n "$1" ]; then
	DOTNET_RUN="${1}";
	shift
fi

if [ -n "$1" ]; then
	DOTNET_LOG_NAME="${1}";
	shift
fi

while [ "$1" != "" ]; do
	PARAMETERS="$PARAMETERS --${1}";
	shift
done

sed -i "s!Server=.*;Pooling=!Server=$BUILD_MYSQL_HOST;Port=3306;Database=${BUILD_MYSQL_DATABASE};User ID=${BUILD_MYSQL_USER};Password=${BUILD_MYSQL_PASSWORD};Pooling=!g" ${PATH_TO_CONF}/appsettings.${BUILD_APP_DOTNET_ENV}.json
sed -i "s!\"base-domain\".*,!\"base-domain\": \"${BUILD_APP_CORE_BASE_DOMAIN}\",!g" ${PATH_TO_CONF}/appsettings.${BUILD_APP_DOTNET_ENV}.json
sed -i "s!\"machinekey\".*,!\"machinekey\": \"${BUILD_APP_CORE_MACHINEKEY}\",!g" ${PATH_TO_CONF}/appsettings.${BUILD_APP_DOTNET_ENV}.json
sed -i "s!\"public\".*,!\"public\": \"${BUILD_APP_URL_PUBLIC}\",!g" ${PATH_TO_CONF}/appsettings.${BUILD_APP_DOTNET_ENV}.json
sed -i "s!\"internal\".*,!\"internal\": \"${BUILD_APP_URL_INTERNAL}\",!g" ${PATH_TO_CONF}/appsettings.${BUILD_APP_DOTNET_ENV}.json
sed -i "s!\"portal\".*,!\"portal\": \"${BUILD_APP_URL_PORTAL}\",!g" ${PATH_TO_CONF}/appsettings.${BUILD_APP_DOTNET_ENV}.json
sed -i "s!\"converter\".*!\"converter\": \"${BUILD_APP_URL_CONVERTER}\"!g" ${PATH_TO_CONF}/appsettings.${BUILD_APP_DOTNET_ENV}.json
sed -i "s!\"viewed-media\".*!\"viewed-media\": \[${BUILD_APP_VIEWED_MEDIA}\]!g" ${PATH_TO_CONF}/appsettings.${BUILD_APP_DOTNET_ENV}.json
sed -i "s!\"exts\".*!\"exts\": \[ ${BUILD_APP_FFMPEG_EXTS} \]!g" ${PATH_TO_CONF}/appsettings.${BUILD_APP_DOTNET_ENV}.json
sed -i "0,/\"value\"/s!\"value\".*,!\"value\": \"${BUILD_APP_JWT_SECRET}\",!" ${PATH_TO_CONF}/appsettings.${BUILD_APP_DOTNET_ENV}.json
sed -i "s!\"header\".*!\"header\": \"${BUILD_APP_SECRET_HEADER}\"!" ${PATH_TO_CONF}/appsettings.${BUILD_APP_DOTNET_ENV}.json

sed -i "s!\"BootstrapServers\".*!\"BootstrapServers\": \"${BUILD_KAFKA_HOST}\"!g" ${PATH_TO_CONF}/kafka.${BUILD_APP_DOTNET_ENV}.json

dotnet ${DOTNET_RUN} --urls=${BUILD_URLS} --ENVIRONMENT=${BUILD_APP_DOTNET_ENV} --'$STORAGE_ROOT'=${BUILD_STORAGE_ROOT} --pathToConf=${PATH_TO_CONF} --log:dir=${LOG_DIR} --log:name=${DOTNET_LOG_NAME} ${PARAMETERS}
