#!/bin/bash

if [[ $EUID -ne 0 ]]; then
    echo "This script must be run as root"
    exit 1
fi

mapfile -t service_name < modules/modules
systemd_catalog=/etc/systemd/system
pid_catalog=/run/appserver

for i in ${!service_name[@]}; do
  echo "disable service_name ${service_name[i]}"
  systemctl stop appserver-${service_name[i]}.service
  systemctl disable appserver-${service_name[i]}.service
done
rm -rf $systemd_catalog/appserver-*.service 
rm -rf $pid_catalog
systemctl daemon-reload
systemctl reset-failed

