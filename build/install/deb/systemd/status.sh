#!/bin/bash

if [[ $EUID -ne 0 ]]; then
    echo "This script must be run as root"
    exit 1
fi

mapfile -t service_name < modules/modules
for i in ${!service_name[@]}; do
  echo "Status ${service_name[i]} - $(systemctl is-active appserver-${service_name[i]}.service)"
done
