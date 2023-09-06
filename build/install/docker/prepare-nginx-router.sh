#!/bin/sh
REDIS_HOST=${REDIS_HOST:-"${REDIS_CONTAINER_NAME}"}
REDIS_PORT=${REDIS_PORT:-"6379"}

envsubst '$MAP_HASH_BUCKET_SIZE,$COUNT_WORKER_CONNECTIONS' < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf
sed -i "s~\(redis_host =\).*~\1 \"$REDIS_HOST\"~" /etc/nginx/conf.d/onlyoffice.conf
sed -i "s~\(redis_port =\).*~\1 $REDIS_PORT~" /etc/nginx/conf.d/onlyoffice.conf
