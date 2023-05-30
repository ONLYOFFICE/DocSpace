#!/bin/bash

set -e

cat<<EOF

#######################################
#  CHECK PORTS
#######################################

EOF

if rpm -qa | grep ${product}; then
	echo "${product} $RES_APP_INSTALLED"
	PRODUCT_INSTALLED="true";
elif [ "${UPDATE}" != "true" ] && netstat -lnp | awk '{print $4}' | grep -qE ":80$|:8081$|:8083$|:5001$|:5002$|:8080$|:80$"; then
	echo "${product} $RES_APP_CHECK_PORTS: 80, 8081, 8083, 5001, 5002, 9200, 2181, 9092";
	echo "$RES_CHECK_PORTS"
	exit
else
	PRODUCT_INSTALLED="false";
fi

if rpm -qa | grep ${package_sysname}-documentserver; then
	echo "${package_sysname}-documentserver $RES_APP_INSTALLED"
	DOCUMENT_SERVER_INSTALLED="true";
elif [ "${UPDATE}" != "true" ] && netstat -lnp | awk '{print $4}' | grep -qE ":8083$|:5432$|:5672$|:6379$|:8000$|:8080$"; then
	echo "${package_sysname}-documentserver $RES_APP_CHECK_PORTS: 8083, 5432, 5672, 6379, 8000, 8080";
	echo "$RES_CHECK_PORTS"
	exit
else
	DOCUMENT_SERVER_INSTALLED="false";
fi

if [ "$PRODUCT_INSTALLED" = "true" ] || [ "$DOCUMENT_SERVER_INSTALLED" = "true" ]; then
	if [ "$UPDATE" != "true" ]; then
		exit;	
	fi
fi
