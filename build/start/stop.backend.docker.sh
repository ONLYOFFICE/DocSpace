#!/bin/bash

echo "Stop all backend services (containers)"
docker stop $(docker ps -a | egrep "onlyoffice" | egrep -v "mysql|rabbitmq|redis|elasticsearch|documentserver" | awk 'NR>0 {print $1}')