#!/bin/bash

SRC_PATH="/AppServer"
ARGS=""

while [ "$1" != "" ]; do
    case $1 in
	    
        -sp | --srcpath )
        	if [ "$2" != "" ]; then
				    SRC_PATH=$2
				    shift
			    fi
		;;

        -ar | --arguments )
          if [ "$2" != "" ]; then
            ARGS=$2
            shift
          fi
    ;;

        -? | -h | --help )
            echo " Usage: bash build-backend.sh [PARAMETER] [[PARAMETER], ...]"
            echo "    Parameters:"
            echo "      -sp, --srcpath             path to AppServer root directory"
            echo "      -ar, --arguments           additional arguments publish the .NET runtime with your application"
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
	
echo "== BACK-END-BUILD =="

cd ${SRC_PATH}
dotnet build ASC.Web.slnf ${ARGS} 


# Array of names backend services in directory common (Nodejs)  
services_name_backend_nodejs=(ASC.Thumbnails)
services_name_backend_nodejs+=(ASC.UrlShortener)
services_name_backend_nodejs+=(ASC.Socket.IO)
services_name_backend_nodejs+=(ASC.SsoAuth)

# Build backend services (Nodejs) 
for i in ${!services_name_backend_nodejs[@]}; do
  echo "== Build ${services_name_backend_nodejs[$i]} project =="
  yarn install --cwd common/${services_name_backend_nodejs[$i]} --frozen-lockfile
done
