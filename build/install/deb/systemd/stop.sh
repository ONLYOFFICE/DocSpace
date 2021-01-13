#!/bin/bash

if [[ $EUID -ne 0 ]]; then
    echo "This script must be run as root"
    exit 1
fi

mapfile -t service_name < modules/modules
for i in ${!service_name[@]}; do
  echo "STOP ${service_name[i]}"
  systemctl stop appserver-${service_name[i]}.service
done
