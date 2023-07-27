#!/bin/bash

set -e

package_manager="yum"
package_sysname="onlyoffice";
product="docspace"
GIT_BRANCH="master"
INSTALLATION_TYPE="ENTERPRISE"
MAKESWAP="true"
RES_APP_INSTALLED="is already installed";
RES_APP_CHECK_PORTS="uses ports"
RES_CHECK_PORTS="please, make sure that the ports are free.";
RES_INSTALL_SUCCESS="Thank you for installing ONLYOFFICE ${product^^}.";
RES_QUESTIONS="In case you have any questions contact us via http://support.onlyoffice.com or visit our forum at http://dev.onlyoffice.org"
RES_MARIADB="To continue the installation, you need to remove MariaDB"

res_unsupported_version () {
	RES_CHOICE="Please, enter Y or N"
	RES_CHOICE_INSTALLATION="Continue installation [Y/N]? "
	RES_UNSPPORTED_VERSION="You have an unsupported version of $DIST installed"
	RES_SELECT_INSTALLATION="Select 'N' to cancel the ONLYOFFICE installation (recommended). Select 'Y' to continue installing ONLYOFFICE"
	RES_ERROR_REMINDER="Please note, that if you continue with the installation, there may be errors"
}

while [ "$1" != "" ]; do
	case $1 in

		-u | --update )
			if [ "$2" != "" ]; then
				UPDATE=$2
				shift
			fi
		;;

		-je | --jwtenabled )
			if [ "$2" != "" ]; then
				JWT_ENABLED=$2
				shift
			fi
		;;

		-jh | --jwtheader )
			if [ "$2" != "" ]; then
				JWT_HEADER=$2
				shift
			fi
		;;

		-js | --jwtsecret )
			if [ "$2" != "" ]; then
				JWT_SECRET=$2
				shift
			fi
		;;
		
		-gb | --gitbranch )
			if [ "$2" != "" ]; then
				PARAMETERS="$PARAMETERS ${1}";
				GIT_BRANCH=$2
				shift
			fi
		;;
		
		-ls | --localscripts )
			if [ "$2" != "" ]; then
				LOCAL_SCRIPTS=$2
				shift
			fi
		;;

		-skiphc | --skiphardwarecheck )
			if [ "$2" != "" ]; then
				SKIP_HARDWARE_CHECK=$2
				shift
			fi
		;;

		-it | --installation_type )
			if [ "$2" != "" ]; then
				INSTALLATION_TYPE=$(echo "$2" | awk '{print toupper($0)}');
				shift
			fi
		;;
		
		-ms | --makeswap )
			if [ "$2" != "" ]; then
				MAKESWAP=$2
				shift
			fi
		;;

		-? | -h | --help )
			echo "  Usage $0 [PARAMETER] [[PARAMETER], ...]"
			echo "    Parameters:"
			echo "      -it, --installation_type          installation type (community|enterprise)"
			echo "      -u, --update                      use to update existing components (true|false)"
			echo "      -je, --jwtenabled                 specifies the enabling the JWT validation (true|false)"
			echo "      -jh, --jwtheader                  defines the http header that will be used to send the JWT"
			echo "      -js, --jwtsecret                  defines the secret key to validate the JWT in the request"
			echo "      -ls, --local_scripts              use 'true' to run local scripts (true|false)"
			echo "      -skiphc, --skiphardwarecheck      use to skip hardware check (true|false)"
			echo "      -ms, --makeswap                   make swap file (true|false)"
			echo "      -?, -h, --help                    this help"
			echo
			exit 0
		;;

	esac
	shift
done

if [ -z "${UPDATE}" ]; then
   UPDATE="false";
fi

if [ -z "${LOCAL_SCRIPTS}" ]; then
   LOCAL_SCRIPTS="false";
fi

if [ -z "${SKIP_HARDWARE_CHECK}" ]; then
   SKIP_HARDWARE_CHECK="false";
fi

cat > /etc/yum.repos.d/onlyoffice.repo <<END
[onlyoffice]
name=onlyoffice repo
baseurl=http://download.onlyoffice.com/repo/centos/main/noarch/
gpgcheck=1
enabled=1
gpgkey=https://download.onlyoffice.com/GPG-KEY-ONLYOFFICE
END

DOWNLOAD_URL_PREFIX="https://raw.githubusercontent.com/ONLYOFFICE/${product}/${GIT_BRANCH}/build/install/OneClickInstall/install-RedHat"

if [ "$LOCAL_SCRIPTS" = "true" ]; then
	source install-RedHat/tools.sh
	source install-RedHat/bootstrap.sh
	source install-RedHat/check-ports.sh
	source install-RedHat/install-preq.sh
	source install-RedHat/install-app.sh
else
	source <(curl ${DOWNLOAD_URL_PREFIX}/tools.sh)
	source <(curl ${DOWNLOAD_URL_PREFIX}/bootstrap.sh)
	source <(curl ${DOWNLOAD_URL_PREFIX}/check-ports.sh)
	source <(curl ${DOWNLOAD_URL_PREFIX}/install-preq.sh)
	source <(curl ${DOWNLOAD_URL_PREFIX}/install-app.sh)
fi
