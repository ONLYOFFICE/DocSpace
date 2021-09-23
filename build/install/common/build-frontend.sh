#!/bin/bash

SRC_PATH="/AppServer"
BUILD_ARGS="build"
DEPLOY_ARGS="deploy"

while [ "$1" != "" ]; do
    case $1 in
	    
        -sp | --srcpath )
        	if [ "$2" != "" ]; then
				SRC_PATH=$2
				shift
			fi
		;;
        -ba | --build-args )
        	if [ "$2" != "" ]; then
				BUILD_ARGS=$2
				shift
			fi
		;;
        -da | --deploy-args )
        	if [ "$2" != "" ]; then
				DEPLOY_ARGS=$2
				shift
			fi
		;;
        -? | -h | --help )
            echo " Usage: bash build-backend.sh [PARAMETER] [[PARAMETER], ...]"
            echo "    Parameters:"
            echo "      -sp, --srcpath             path to AppServer root directory"
            echo "      -ba, --build-args          arguments for yarn building"
            echo "      -da, --deploy-args         arguments for yarn deploying"
            echo "      -?, -h, --help             this help"
            echo "  Examples"
            echo "  bash build-backend.sh -sp /app/AppServer"
            exit 0
    ;;

		* )
			echo "Unknown parameter $1" 1>&2
			exit 1
		;;
    esac
  shift
done

echo "== FRONT-END-BUILD =="

cd ${SRC_PATH}
yarn install
yarn ${BUILD_ARGS}
yarn ${DEPLOY_ARGS}
