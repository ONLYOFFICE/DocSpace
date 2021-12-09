#!/bin/sh
export NGINX_WORKER_PROCESSES=${NGINX_WORKER_PROCESSES:-$(grep processor /proc/cpuinfo | wc -l)};
export NGINX_WORKER_CONNECTIONS=${NGINX_WORKER_CONNECTIONS:-4096};

envsubst '$MAP_HASH_BUCKET_SIZE,$NGINX_WORKER_PROCESSES,$NGINX_WORKER_CONNECTIONS' < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf
