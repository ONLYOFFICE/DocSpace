#!/bin/bash 

PATH_TO_CONF=${PATH_TO_CONF:-"/app/onlyoffice/config"}
APP_CORE_BASE_DOMAIN=${APP_CORE_BASE_DOMAIN:-"localhost"}
APP_URL_PORTAL=${APP_URL_PORTAL:-"http://127.0.0.1:8092"}

APP_CORE_MACHINEKEY=${APP_CORE_MACHINEKEY:-"your_core_machinekey"}
DOCUMENT_CONTAINER_NAME=${DOCUMENT_CONTAINER_NAME:-"onlyoffice-document-server"}
DOCUMENT_SERVER_URL_PUBLIC=${DOCUMENT_SERVER_URL_PUBLIC:-"/ds-vpath/"}
DOCUMENT_SERVER_URL_EXTERNAL=${DOCUMENT_SERVER_URL_EXTERNAL:-"http://${DOCUMENT_CONTAINER_NAME}/"}
DOCUMENT_SERVER_JWT_SECRET=${DOCUMENT_SERVER_JWT_SECRET:-"your_jwt_secret"}
DOCUMENT_SERVER_JWT_HEADER=${DOCUMENT_SERVER_JWT_HEADER:-"AuthorizationJwt"}

MYSQL_CONTAINER_NAME=${MYSQL_CONTAINER_NAME:-"onlyoffice-mysql-server"}
MYSQL_HOST=${MYSQL_HOST:-${MYSQL_CONTAINER_NAME}}
MYSQL_PORT=${MYSQL_PORT:-"3306"}
MYSQL_DATABASE=${MYSQL_DATABASE:-"docspace"}
MYSQL_USER=${MYSQL_USER:-"onlyoffice_user"}
MYSQL_PASSWORD=${MYSQL_PASSWORD:-"onlyoffice_pass"}

RABBIT_CONNECTION_HOST=${RABBIT_CONNECTION_HOST:-"onlyoffice-rabbitmq"}
REDIS_CONNECTION_HOST=${REDIS_CONNECTION_HOST:-"onlyoffice-redis"}

ELK_CONNECTION_HOST=${ELK_CONNECTION_HOST:-"onlyoffice-opensearch"}
ELK_SHEME=${ELK_SHEME:-"http"}
ELK_HOST=${ELK_HOST:-${ELK_CONNECTION_HOST}}
ELK_PORT=${ELK_PORT:-"9200"}
ELK_THREADS=${ELK_THREADS:-"1"}

sed -i "s!\"connectionString\".*;Pooling=!\"connectionString\": \"Server=${MYSQL_HOST};Port=${MYSQL_PORT};Database=${MYSQL_DATABASE};User ID=${MYSQL_USER};Password=${MYSQL_PASSWORD};Pooling=!g" ${PATH_TO_CONF}/appsettings.json
sed -i "s!\"connectionString\".*;Pooling=!\"connectionString\": \"Server=${MYSQL_HOST};Port=${MYSQL_PORT};Database=${MYSQL_DATABASE};User ID=${MYSQL_USER};Password=${MYSQL_PASSWORD};Pooling=!g" ${PATH_TO_CONF}/apisystem.json
sed -i "s!\"base-domain\".*,!\"base-domain\": \"${APP_CORE_BASE_DOMAIN}\",!g" ${PATH_TO_CONF}/appsettings.json
sed -i "s!\"base-domain\".*,!\"base-domain\": \"${APP_CORE_BASE_DOMAIN}\",!g" ${PATH_TO_CONF}/apisystem.json
sed -i "s!\"machinekey\".*,!\"machinekey\": \"${APP_CORE_MACHINEKEY}\",!g" ${PATH_TO_CONF}/appsettings.json
sed -i "s!\"machinekey\".*,!\"machinekey\": \"${APP_CORE_MACHINEKEY}\",!g" ${PATH_TO_CONF}/apisystem.json
sed -i "s!\"public\".*,!\"public\": \"${DOCUMENT_SERVER_URL_PUBLIC}\",!g" ${PATH_TO_CONF}/appsettings.json
sed -i "s!\"internal\".*,!\"internal\": \"${DOCUMENT_SERVER_URL_EXTERNAL}\",!g" ${PATH_TO_CONF}/appsettings.json
sed -i "0,/\"value\"/s!\"value\".*,!\"value\": \"${DOCUMENT_SERVER_JWT_SECRET}\",!" ${PATH_TO_CONF}/appsettings.json
sed -i "s!\"portal\".*!\"portal\": \"${APP_URL_PORTAL}\"!g" ${PATH_TO_CONF}/appsettings.json

sed -i "s!\"Hostname\".*!\"Hostname\": \"${RABBIT_CONNECTION_HOST}\",!g" ${PATH_TO_CONF}/rabbitmq.json
sed -i "s!\"Host\".*!\"Host\": \"${REDIS_CONNECTION_HOST}\",!g" ${PATH_TO_CONF}/redis.json
sed -i "s!\"elastic\".*{!\"elastic\": {\n\"Scheme\": \"${ELK_SHEME}\",\n\"Host\": \"${ELK_HOST}\",\n\"Port\": \"${ELK_PORT}\",\n\"Threads\": \"${ELK_THREADS}\"!g" ${PATH_TO_CONF}/elastic.json

supervisord -n