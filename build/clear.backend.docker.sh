#!/bin/bash

echo "Stop all onlyoffice containers."
docker stop $(docker ps -a | egrep "onlyoffice" | awk 'NR>0 {print $1}')
echo "Remove all onlyoffice containers."
docker rm -f $(docker ps -a | egrep "onlyoffice" | awk 'NR>0 {print $1}')
echo "Remove all onlyoffice images."
docker rmi -f $(docker images -a | egrep "onlyoffice" | awk 'NR>0 {print $3}')
echo "Remove unused volumes."
docker volume prune -f
echo "Remove unused networks."
docker network prune -f