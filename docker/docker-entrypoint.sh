#!/bin/bash 

PATH_TO_CONF=${PATH_TO_CONF:-"/app/onlyoffice/config"}
SRC_PATH=${SRC_PATH:-"/app/onlyoffice/src"}
APP_CORE_BASE_DOMAIN=${APP_CORE_BASE_DOMAIN:-"localhost"}
APP_URL_PORTAL=${APP_URL_PORTAL:-"http://127.0.0.1:8092"}

APP_CORE_MACHINEKEY=${APP_CORE_MACHINEKEY:-"your_core_machinekey"}
DOCUMENT_CONTAINER_NAME=${DOCUMENT_CONTAINER_NAME:-"onlyoffice-document-server"}
DOCUMENT_SERVER_URL_PUBLIC=${DOCUMENT_SERVER_URL_PUBLIC:-"/ds-vpath/"}
DOCUMENT_SERVER_URL_EXTERNAL=${DOCUMENT_SERVER_URL_EXTERNAL:-"http://${DOCUMENT_CONTAINER_NAME}"}
DOCUMENT_SERVER_JWT_SECRET=${DOCUMENT_SERVER_JWT_SECRET:-"your_jwt_secret"}
DOCUMENT_SERVER_JWT_HEADER=${DOCUMENT_SERVER_JWT_HEADER:-"AuthorizationJwt"}

MYSQL_CONTAINER_NAME=${MYSQL_CONTAINER_NAME:-"onlyoffice-mysql-server"}
MYSQL_HOST=${MYSQL_HOST:-${MYSQL_CONTAINER_NAME}}
MYSQL_PORT=${MYSQL_PORT:-"3306"}
MYSQL_DATABASE=${MYSQL_DATABASE:-"docspace"}
MYSQL_USER=${MYSQL_USER:-"onlyoffice_user"}
MYSQL_PASSWORD=${MYSQL_PASSWORD:-"onlyoffice_pass"}
COMMAND_TIMEOUT=${COMMAND_TIMEOUT:-"100"}

RABBIT_CONNECTION_HOST=${RABBIT_CONNECTION_HOST:-"onlyoffice-rabbitmq"}
RABBIT_HOST=${RABBIT_HOST:-${RABBIT_CONNECTION_HOST}}

REDIS_CONNECTION_HOST=${REDIS_CONNECTION_HOST:-"onlyoffice-redis"}
REDIS_HOST=${REDIS_HOST:-${REDIS_CONNECTION_HOST}}

# Configure custom parameters 
sed -i "s!\"connectionString\".*;Pooling=!\"connectionString\": \"Server=${MYSQL_HOST};Port=${MYSQL_PORT};Database=${MYSQL_DATABASE};User ID=${MYSQL_USER};Password=${MYSQL_PASSWORD};Pooling=!g" ${PATH_TO_CONF}/appsettings.json
sed -i "s!\"connectionString\".*;Pooling=!\"connectionString\": \"Server=${MYSQL_HOST};Port=${MYSQL_PORT};Database=${MYSQL_DATABASE};User ID=${MYSQL_USER};Password=${MYSQL_PASSWORD};Pooling=!g" ${PATH_TO_CONF}/apisystem.json
sed -i "s!\"ConnectionString\".*!\"ConnectionString\": \"Server=${MYSQL_HOST};Port=${MYSQL_PORT};Database=${MYSQL_DATABASE};User ID=${MYSQL_USER};Password=${MYSQL_PASSWORD};Command Timeout=${COMMAND_TIMEOUT}\"!g" ${SRC_PATH}/publish/services/ASC.Migration.Runner/service/appsettings.runner.json
sed -i "s!\"base-domain\".*,!\"base-domain\": \"${APP_CORE_BASE_DOMAIN}\",!g" ${PATH_TO_CONF}/appsettings.json
sed -i "s!\"base-domain\".*,!\"base-domain\": \"${APP_CORE_BASE_DOMAIN}\",!g" ${PATH_TO_CONF}/apisystem.json
sed -i "s!\"machinekey\".*,!\"machinekey\": \"${APP_CORE_MACHINEKEY}\",!g" ${PATH_TO_CONF}/appsettings.json
sed -i "s!\"machinekey\".*,!\"machinekey\": \"${APP_CORE_MACHINEKEY}\",!g" ${PATH_TO_CONF}/apisystem.json
sed -i "s!\"public\".*,!\"public\": \"${DOCUMENT_SERVER_URL_PUBLIC}\",!g" ${PATH_TO_CONF}/appsettings.json
sed -i "s!\"internal\".*,!\"internal\": \"${DOCUMENT_SERVER_URL_EXTERNAL}/\\\",!g" ${PATH_TO_CONF}/appsettings.json
sed -i "0,/\"value\"/s!\"value\".*,!\"value\": \"${DOCUMENT_SERVER_JWT_SECRET}\",!" ${PATH_TO_CONF}/appsettings.json
sed -i "s!\"portal\".*!\"portal\": \"${APP_URL_PORTAL}\"!g" ${PATH_TO_CONF}/appsettings.json

sed -i "s!\"Hostname\".*!\"Hostname\": \"${RABBIT_HOST}\",!g" ${PATH_TO_CONF}/rabbitmq.json
sed -i "s!\"Host\".*!\"Host\": \"${REDIS_HOST}\",!g" ${PATH_TO_CONF}/redis.json

supervisord -n
