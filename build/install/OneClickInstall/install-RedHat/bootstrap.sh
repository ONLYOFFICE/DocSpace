#!/bin/bash

set -e

cat<<EOF

#######################################
#  BOOTSTRAP
#######################################

EOF

if ! rpm -q net-tools; then
	${package_manager} -y install net-tools;
fi
