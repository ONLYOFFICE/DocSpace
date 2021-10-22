#!/bin/bash

# read parameters
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

echo "#-------------------------------------#"
echo "Run ${DOTNET_RUN}"
echo "#-------------------------------------#"
    
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

ELK_SHEME=${ELK_SHEME:-"http"}
ELK_HOST=${ELK_HOST:-"${PRODUCT}-elasticsearch"}
ELK_PORT=${ELK_PORT:-"9200"}
ELK_THREADS=${ELK_THREADS:-"1"}

KAFKA_HOST=${KAFKA_HOST:-"kafka"}":9092"

APP_STORAGE_ROOT=${APP_STORAGE_ROOT:-"${BASE_DIR}/data/"}

sed -i "s!Server=.*;Pooling=!Server=${MYSQL_HOST};Port=3306;Database=${MYSQL_DATABASE};User ID=${MYSQL_USER};Password=${MYSQL_PASSWORD};Pooling=!g" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "s!\"base-domain\".*,!\"base-domain\": \"${APP_CORE_BASE_DOMAIN}\",!g" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "s!\"machinekey\".*,!\"machinekey\": \"${APP_CORE_MACHINEKEY}\",!g" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "s!\"public\".*,!\"public\": \"${DOCUMENT_SERVER_URL_PUBLIC}\",!g" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "s!\"internal\".*,!\"internal\": \"${DOCUMENT_SERVER_URL_INTERNAL}\",!g" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "s!\"portal\".*!\"portal\": \"${APP_URL_PORTAL}\",!g" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "0,/\"value\"/s!\"value\".*,!\"value\": \"${DOCUMENT_SERVER_JWT_SECRET}\",!" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json
sed -i "s!\"header\".*!\"header\": \"${DOCUMENT_SERVER_JWT_HEADER}\"!" ${PATH_TO_CONF}/appsettings.${APP_DOTNET_ENV}.json

sed -i "s!\"Scheme\".*!\"Scheme\": \"${ELK_SHEME}\",!g" ${PATH_TO_CONF}/elastic.json
sed -i "s!\"Host\".*!\"Host\": \"${ELK_HOST}\",!g" ${PATH_TO_CONF}/elastic.json
sed -i "s!\"Port\".*!\"Port\": \"${ELK_PORT}\",!g" ${PATH_TO_CONF}/elastic.json
sed -i "s!\"Threads\".*!\"Threads\": \"${ELK_THREADS}\"!g" ${PATH_TO_CONF}/elastic.json

sed -i "s!\"subfolder\".*!\"subfolder\": \"server\",!g" ${PATH_TO_CONF}/appsettings.services.json
sed -i "s!\"BootstrapServers\".*!\"BootstrapServers\": \"${KAFKA_HOST}\"!g" ${PATH_TO_CONF}/kafka.${APP_DOTNET_ENV}.json

dotnet ${DOTNET_RUN} --urls=${URLS} --ENVIRONMENT=${APP_DOTNET_ENV} --'$STORAGE_ROOT'=${APP_STORAGE_ROOT} --pathToConf=${PATH_TO_CONF} --log:dir=${LOG_DIR} --log:name=${DOTNET_LOG_NAME} ${PARAMETERS}
