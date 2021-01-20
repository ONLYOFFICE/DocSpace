#!/bin/bash

echo "##########################################################"
echo "##############    Run App Service     ####################"
echo "##########################################################"
    
PRODUCT=${PRODUCT:-"onlyoffice"}
BASE_DIR="/app/${PRODUCT}"
PARAMETERS=""
PROXY_HOST=${PROXY_HOST:-"proxy"}
SHEME=${SHEME:-"http"}
SERVICE_PORT=${SERVICE_PORT:-"5050"}

URLS=${URLS:-"${SHEME}://0.0.0.0:${SERVICE_PORT}"}
PATH_TO_CONF=${PATH_TO_CONF:-"${BASE_DIR}/config"}
LOG_DIR=${LOG_DIR:-"/var/log/${PRODUCT}"}

MYSQL_HOST=${MYSQL_HOST:-"mysql-server"}
MYSQL_DATABASE=${MYSQL_DATABASE:-${PRODUCT}}
MYSQL_USER=${MYSQL_USER:-"${PRODUCT}_user"}
MYSQL_PASSWORD=${MYSQL_PASSWORD:-"${PRODUCT}_pass"}

APP_DOTNET_ENV=${APP_DOTNET_ENV:-"test"}
APP_CORE_BASE_DOMAIN=${APP_CORE_BASE_DOMAIN:-"localhost"}
APP_URL_PORTAL=${APP_URL_PORTAL:-"${SHEME}://${PROXY_HOST}:8092"}

APP_CORE_MACHINEKEY=${APP_CORE_MACHINEKEY:-"your_core_machinekey"}
DOCUMENT_SERVER_JWT_SECRET=${DOCUMENT_SERVER_JWT_SECRET:-"your_jwt_secret"}
DOCUMENT_SERVER_JWT_HEADER=${DOCUMENT_SERVER_JWT_HEADER:-"AuthorizationJwt"}
DOCUMENT_SERVER_URL_PUBLIC=${DOCUMENT_SERVER_URL_PUBLIC:-"/ds-vpath/"}
DOCUMENT_SERVER_URL_INTERNAL=${DOCUMENT_SERVER_URL_INTERNAL:-"${SHEME}://${PRODUCT}-document-server/"}
DOCUMENT_SERVER_URL_CONVERTER=${DOCUMENT_SERVER_URL_CONVERTER:-"/ds-vpath/ConvertService.ashx"}
VIEWED_MEDIA=${VIEWED_MEDIA:-'".aac",".flac",".m4a",".mp3",".oga",".ogg",".wav",".f4v",".m4v",".mov",".mp4",".ogv",".webm",".avi"'}
FFMPEG_EXTS=${FFMPEG_EXTS:-'"avi", "mpeg", "mpg", "wmv"'}

ELK_SHEME=${ELK_SHEME:-"http"}
ELK_HOST=${ELK_HOST:-"elasticsearch"}
ELK_PORT=${ELK_PORT:-"9200"}
ELK_VALUE='"elastic": { "Scheme": "'${ELK_SHEME}'", "Host": "'${ELK_HOST}'", "Port": "'${ELK_PORT}'" },'

KAFKA_HOST=${KAFKA_HOST:-"kafka"}":9092"

APP_STORAGE_ROOT=${APP_STORAGE_ROOT:-"${BASE_DIR}/data/"}

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

sed -i "s!Server=.*;Pooling=!Server=${MYSQL_HOST};Port=3306;Database=${MYSQL_DATABASE};User ID=${MYSQL_USER};Password=${MYSQL_PASSWORD};Pooling=!g" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "s!\"base-domain\".*,!\"base-domain\": \"${APP_CORE_BASE_DOMAIN}\",!g" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "s!\"machinekey\".*,!\"machinekey\": \"${APP_CORE_MACHINEKEY}\",!g" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "s!\"public\".*,!\"public\": \"${DOCUMENT_SERVER_URL_PUBLIC}\",!g" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "s!\"internal\".*,!\"internal\": \"${DOCUMENT_SERVER_URL_INTERNAL}\",!g" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "s!\"portal\".*!\"portal\": \"${APP_URL_PORTAL}\",!g" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "s!\"viewed-media\".*!\"viewed-media\": \[${VIEWED_MEDIA}\]!g" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "s!\"exts\".*!\"exts\": \[ ${FFMPEG_EXTS} \]!g" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "0,/\"value\"/s!\"value\".*,!\"value\": \"${DOCUMENT_SERVER_JWT_SECRET}\",!" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "s!\"header\".*!\"header\": \"${DOCUMENT_SERVER_JWT_HEADER}\"!" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json

grep -q "${ELK_VALUE}" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json || sed -i "s!\"files\".*!${ELK_VALUE}\n\"files\": {!" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json

sed -i "s!\"BootstrapServers\".*!\"BootstrapServers\": \"${KAFKA_HOST}\"!g" ${PATH_TO_CONF}/kafka.${APP_DOTNET_ENV}.json

S3_STORAGE=${S3_STORAGE:-"false"}

if [ "${S3_STORAGE}" == "true" ]; then
	
	S3_PATH_TO_CONF=${PATH_TO_CONF}/autofac.consumers.json

	S3_ACCESSKEY=${S3_ACCESSKEY:-""}
	S3_SECRETACCESSKEY=${S3_SECRETACCESSKEY:-""}
	S3_BUCKET=${S3_BUCKET:-""}
	S3_REGION=${S3_REGION:-""}
	S3_SERVICEURL=${S3_SERVICEURL:-""}
	S3_FORCEPATHSTYLE=${S3_FORCEPATHSTYLE:-""}
	S3_USEHTTP=${S3_USEHTTP:-""}
	S3_SEE=${S3_SEE:-""}
    
	jq '.components[18].parameters.props.acesskey="'${S3_ACCESSKEY}'"' ${S3_PATH_TO_CONF} > ${S3_PATH_TO_CONF}.tmp && mv ${S3_PATH_TO_CONF}.tmp ${S3_PATH_TO_CONF}
	jq '.components[18].parameters.props.secretaccesskey="'${S3_SECRETACCESSKEY}'"' ${S3_PATH_TO_CONF} > ${S3_PATH_TO_CONF}.tmp && mv ${S3_PATH_TO_CONF}.tmp ${S3_PATH_TO_CONF}
	jq '.components[18].parameters.additional.bucket="'${S3_BUCKET}'"' ${S3_PATH_TO_CONF} > ${S3_PATH_TO_CONF}.tmp && mv ${S3_PATH_TO_CONF}.tmp ${S3_PATH_TO_CONF}
	jq '.components[18].parameters.additional.region="'${S3_REGION}'"' ${S3_PATH_TO_CONF} > ${S3_PATH_TO_CONF}.tmp && mv ${S3_PATH_TO_CONF}.tmp ${S3_PATH_TO_CONF}
	jq '.components[18].parameters.additional.serviceurl="'${S3_SERVICEURL}'"' ${S3_PATH_TO_CONF} > ${S3_PATH_TO_CONF}.tmp && mv ${S3_PATH_TO_CONF}.tmp ${S3_PATH_TO_CONF}
	jq '.components[18].parameters.additional.forcepathstyle="'${S3_FORCEPATHSTYLE}'"' ${S3_PATH_TO_CONF} > ${S3_PATH_TO_CONF}.tmp && mv ${S3_PATH_TO_CONF}.tmp ${S3_PATH_TO_CONF}
	jq '.components[18].parameters.additional.usehttp="'${S3_USEHTTP}'"' ${S3_PATH_TO_CONF} > ${S3_PATH_TO_CONF}.tmp && mv ${S3_PATH_TO_CONF}.tmp ${S3_PATH_TO_CONF}
	jq '.components[18].parameters.additional.sse="'${S3_SEE}'"' ${S3_PATH_TO_CONF} > ${S3_PATH_TO_CONF}.tmp && mv ${S3_PATH_TO_CONF}.tmp ${S3_PATH_TO_CONF}
fi

dotnet ${DOTNET_RUN} --urls=${URLS} --ENVIRONMENT=${APP_DOTNET_ENV} --'$STORAGE_ROOT'=${APP_STORAGE_ROOT} --pathToConf=${PATH_TO_CONF} --log:dir=${LOG_DIR} --log:name=${DOTNET_LOG_NAME} ${PARAMETERS}
