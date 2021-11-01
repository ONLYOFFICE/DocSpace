#!/bin/sh
envsubst '$MAP_HASH_BUCKET_SIZE,$COUNT_WORKER_CONNECTIONS' < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf
