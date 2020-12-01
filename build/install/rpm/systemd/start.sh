#!/bin/bash

if [[ $EUID -ne 0 ]]; then
    echo "This script must be run as root"
    exit 1
fi

mapfile -t service_name < modules/modules
systemd_catalog=/etc/systemd/system
pid_catalog=/run/appserver

cp modules/appserver-*.service $systemd_catalog
mkdir $pid_catalog

for i in ${!service_name[@]}; do
	touch $pid_catalog/appserver-${service_name[i]}.pid
  echo "Start ${service_name[i]}"
  systemctl start appserver-${service_name[i]}.service
done
