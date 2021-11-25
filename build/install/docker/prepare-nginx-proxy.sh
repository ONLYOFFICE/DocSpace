#!/bin/sh
UPSTREAM=${UPSTREAM:-"true"}

envsubst '$MAP_HASH_BUCKET_SIZE,$COUNT_WORKER_CONNECTIONS' < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf

if [ ${UPSTREAM} = "true" ]; then
    cp /etc/nginx/upstream.conf.template /etc/nginx/conf.d/upstream.conf
fi

if [ ${UPSTREAM} = "false" ]; then
    cp /etc/nginx/map.conf.template /etc/nginx/conf.d/map.conf
fi
