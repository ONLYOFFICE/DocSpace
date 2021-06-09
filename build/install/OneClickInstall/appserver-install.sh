#!/bin/bash

#
#
# (c) Copyright Ascensio System Limited 2010-2021
#
# This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
# General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
# In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
# Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
#
# THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
# FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
#
# You can contact Ascensio System SIA by email at sales@onlyoffice.com
#
# The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
# Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
#
# Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
# relevant author attributions when distributing the software. If the display of the logo in its graphic 
# form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
# in every copy of the program you distribute. 
# Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
#
#

PARAMETERS="";
DOCKER="";
LOCAL_SCRIPTS="false"
HELP="false";
product="appserver"

while [ "$1" != "" ]; do
	case $1 in
		-ls | --local_scripts )
			if [ "$2" == "true" ] || [ "$2" == "false" ]; then
				PARAMETERS="$PARAMETERS ${1}";
				LOCAL_SCRIPTS=$2
				shift
			fi
		;;

		"-?" | -h | --help )
			HELP="true";
			DOCKER="true";
			PARAMETERS="$PARAMETERS -ht install-Docker.sh";
		;;
	esac
	PARAMETERS="$PARAMETERS ${1}";
	shift
done

root_checking () {
	if [ ! $( id -u ) -eq 0 ]; then
		echo "To perform this action you must be logged in with root rights"
		exit 1;
	fi
}

command_exists () {
	type "$1" &> /dev/null;
}

install_curl () {
	if command_exists apt-get; then
		apt-get -y update
		apt-get -y -q install curl
	elif command_exists yum; then
		yum -y install curl
	fi

	if ! command_exists curl; then
		echo "command curl not found"
		exit 1;
	fi
}

read_installation_method () {
	echo "Select 'Y' to install ONLYOFFICE $product using Docker (recommended). Select 'N' to install it using RPM/DEB packages.";
	read -p "Install with Docker [Y/N/C]? " choice
	case "$choice" in
		y|Y )
			DOCKER="true";
		;;

		n|N )
			DOCKER="false";
		;;

		c|C )
			exit 0;
		;;

		* )
			echo "Please, enter Y, N or C to cancel";
		;;
	esac

	if [ "$DOCKER" == "" ]; then
		read_installation_method;
	fi
}

root_checking

if ! command_exists curl ; then
	install_curl;
fi

if [ "$HELP" == "false" ]; then
	read_installation_method;
fi

#DOWNLOAD_URL_PREFIX="http://download.onlyoffice.com/install-appserver/"
DOWNLOAD_URL_PREFIX="https://raw.githubusercontent.com/ONLYOFFICE/${product}/develop/build/install/OneClickInstall"

if [ "$DOCKER" == "true" ]; then
	if [ "$LOCAL_SCRIPTS" == "true" ]; then
		bash install-Docker.sh ${PARAMETERS}
	else
		curl -s -O  ${DOWNLOAD_URL_PREFIX}/install-Docker.sh
		bash install-Docker.sh ${PARAMETERS}
		rm install-Docker.sh
	fi
else
	if [ -f /etc/redhat-release ] ; then
		DIST=$(cat /etc/redhat-release |sed s/\ release.*//);
		REV=$(cat /etc/redhat-release | sed s/.*release\ // | sed s/\ .*//);

		REV_PARTS=(${REV//\./ });
		REV=${REV_PARTS[0]};

		if [[ "${DIST}" == CentOS* ]] && [ ${REV} -lt 7 ]; then
			echo "CentOS 7 or later is required";
			exit 1;
		fi
		if [ "$LOCAL_SCRIPTS" == "true" ]; then
			bash install-RedHat.sh ${PARAMETERS}
		else
			curl -s -O ${DOWNLOAD_URL_PREFIX}/install-RedHat.sh
			bash install-RedHat.sh ${PARAMETERS}
			rm install-RedHat.sh
		fi
	elif [ -f /etc/debian_version ] ; then
		if [ "$LOCAL_SCRIPTS" == "true" ]; then
			bash install-Debian.sh ${PARAMETERS}
		else
			curl -s -O ${DOWNLOAD_URL_PREFIX}/install-Debian.sh
			bash install-Debian.sh ${PARAMETERS}
			rm install-Debian.sh
		fi
	else
		echo "Not supported OS";
		exit 1;
	fi
fi
