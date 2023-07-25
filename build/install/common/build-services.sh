#!/bin/bash
set -xe

PROJECT_REPOSITORY_NAME=${PROJECT_REPOSITORY_NAME:-"DocSpace"}
SRC_PATH=${SRC_PATH:-$(pwd | sed "s/${PROJECT_REPOSITORY_NAME}.*/${PROJECT_REPOSITORY_NAME}/g")}
BUILD_PATH=${BUILD_PATH:-${SRC_PATH}/publish}
BUILD_DOTNET_CORE_ARGS=${BUILD_DOTNET_CORE_ARGS:-"false"}
PROPERTY_BUILD=${PROPERTY_BUILD:-"all"}

BACKEND_NODEJS_SERVICES=${BACKEND_NODEJS_SERVICES:-"ASC.Socket.IO, ASC.SsoAuth"}
BACKEND_DOTNETCORE_SERVICES=${BACKEND_DOTNETCORE_SERVICES:-"ASC.Files, ASC.People, ASC.Data.Backup, ASC.Files.Service, ASC.Notify, \
ASC.Studio.Notify, ASC.Web.Api, ASC.Web.Studio, ASC.Data.Backup.BackgroundTasks, ASC.ClearEvents, ASC.ApiSystem, ASC.Web.HealthChecks.UI"}
SELF_CONTAINED=${SELF_CONTAINED:-"false"}
PUBLISH_BACKEND_ARGS=${PUBLISH_BACKEND_ARGS:-"false"}
PUBLISH_CNF=${PUBLISH_CNF:-"Release"}

FRONTEND_BUILD_ARGS=${FRONTEND_BUILD_ARGS:-"build"}
FRONTEND_DEPLOY_ARGS=${FRONTEND_DEPLOY_ARGS:-"deploy"}
DEBUG_INFO_CHECK=${DEBUG_INFO_CHECK:-""}
MIGRATION_CHECK=${MIGRATION_CHECK:-"true"}
DOCKER_ENTRYPOINT=${DOCKER_ENTRYPOINT:-"false"}

ARRAY_NAME_SERVICES=()

while [ "$1" != "" ]; do
    case $1 in
        -sp | --srcpath )
        	if [[ "$2" != "" && ! "$2" =~ ^- ]]; then
            SRC_PATH=$2
            BUILD_PATH=${SRC_PATH}/publish
		    		shift
		    	fi
		  ;;
        -bp | --buildpath )
          if [[ "$2" != "" && ! "$2" =~ ^- ]]; then
            BUILD_PATH=$2
            shift
          fi
      ;;
        -pb | --property-build )
          if [[ "$2" != "" && ! "$2" =~ ^- ]]; then
            PROPERTY_BUILD=$2
            shift
          fi
      ;;
        -sc | --self-contained )
          if [[ "$2" != "" && ! "$2" =~ ^- ]]; then
            SELF_CONTAINED=$2
            shift
          fi
      ;;
        -pc | --publish-configuration )
          if [[ "$2" != "" && ! "$2" =~ ^- ]]; then
            PUBLISH_CNF=$2
            shift
          fi
      ;;
        -yb | --frontend-build-args )
          if [[ "$2" != "" && ! "$2" =~ ^- ]]; then
            FRONTEND_BUILD_ARGS=$2
            shift
          fi
      ;;
        -yd | --frontend-deploy-args )
          if [[ "$2" != "" && ! "$2" =~ ^- ]]; then
            FRONTEND_DEPLOY_ARGS=$2
            shift
          fi
      ;;
        -dc | --debug-check )
          if [[ "$2" != "" && ! "$2" =~ ^- ]]; then
            DEBUG_INFO_CHECK=$2
            shift
          fi
      ;;
        -mc | --migration-check )
          if [[ "$2" != "" && ! "$2" =~ ^- ]]; then
            MIGRATION_CHECK=$2
            shift
          fi
      ;;
        -de | --docker-entrypoint )
          if [[ "$2" != "" && ! "$2" =~ ^- ]]; then
            DOCKER_ENTRYPOINT=$2
            shift
          fi
      ;;
        -? | -h | --help )
            echo " Usage: bash build-services.sh [PARAMETER] [[PARAMETER], ...]"
            echo "    Parameters:"
            echo -e " -sp, --srcpath \t path to project root directory"
            echo -e " -bp, --buildpath \t path where generated output is placed (by default={SRC_PATH}/publish)"
            echo -e " -st, --status \t build status Ex. all/frontend-build/backend-publish/backend-dotnet-publish/backend-nodejs-publish/backend-build"
            echo -e " -sc, --self-contained \t publish the .NET runtime with your application (by default=false)"
            echo -e " -pc, --publish-configuration \t dotnet publish configuration Ex. Release/Debug"
            echo -e " -yb, --frontend-build-args \t arguments for yarn building"
            echo -e " -yd, --frontend-deploy-args \t arguments for yarn deploy"
            echo -e " -dc, --debug-check \t arguments for yarn debug info configure"
            echo -e " -mc, --migration-check \t check migration build (by default=true)"
            echo " -?, -h, --help              this help"
            echo "  Examples"
            echo "  bash build-services.sh -sp /app/DocSpace"
            exit 0
      ;;
		    * )
			  echo "Unknown parameter $1" 1>&2
			  exit 1
		  ;;
    esac
  shift
done

cd ${SRC_PATH}
function get_services_name {
  if [[ $# -gt 0 ]]
  then
    ARRAY_NAME_SERVICES=($(echo $1 | tr "," " "))
  fi
}

#  Builds a project dotnetcore dependencies
function build_dotnetcore_backend {
  if [[ ${BUILD_DOTNET_CORE_ARGS} == "false" ]]
  then
    echo "== Build ASC.Web.slnf =="
    dotnet build ASC.Web.slnf
  else
    echo "== Build ASC.Web.slnf ${BUILD_DOTNET_CORE_ARGS} =="
    dotnet build ASC.Web.slnf ${BUILD_DOTNET_CORE_ARGS}
  fi
  
  if [[ $# -gt 0 ]]
  then
    local migration_check=$(echo $1 | tr '[:upper:]' '[:lower:]' | tr -d ' ')
    if [[ ${migration_check} == "true" ]]
    then
      echo "== Build ASC.Migrations.sln =="
      dotnet build ASC.Migrations.sln -o ${BUILD_PATH}/services/ASC.Migration.Runner/service/
    fi
    if [[ ${DOCKER_ENTRYPOINT} != "false" ]]
    then
       echo "== ADD ${SRC_PATH}/build/install/docker/docker-migration-entrypoint.sh to ASC.Migration.Runner =="
       cp ${SRC_PATH}/build/install/docker/docker-migration-entrypoint.sh ${BUILD_PATH}/services/ASC.Migration.Runner/service/
    fi
  fi
}

# Publish BACKEND dotnetcore services
function backend-dotnet-publish {
  # List of names for nodejs backend projects
  get_services_name "${BACKEND_DOTNETCORE_SERVICES}"
  
  echo "== Publish ASC.Web.slnf =="

  if [[ ${PUBLISH_BACKEND_ARGS} == "false" ]]
  then
    dotnet publish $SRC_PATH/ASC.Web.slnf -p "PublishProfile=FolderProfile"
  else
    dotnet publish $SRC_PATH/ASC.Web.slnf ${PUBLISH_BACKEND_ARGS} -p "PublishProfile=FolderProfile"
  fi

  if [[ ${DOCKER_ENTRYPOINT} != "false" ]]
  then
    for i in ${!ARRAY_NAME_SERVICES[@]}; do
      echo "== ADD ${DOCKER_ENTRYPOINT} to ${ARRAY_NAME_SERVICES[$i]} =="
      cp ${DOCKER_ENTRYPOINT} ${BUILD_PATH}/services/${ARRAY_NAME_SERVICES[$i]}/service/
    done
  fi  

  ARRAY_NAME_SERVICES=()
}

# Install BACKEND dependencies for nodjs's projects
function backend-nodejs-publish {
  # List of names for nodejs backend projects
  get_services_name "${BACKEND_NODEJS_SERVICES}"
  for i in ${!ARRAY_NAME_SERVICES[@]}; do
    echo "== Build ${ARRAY_NAME_SERVICES[$i]} project =="
    yarn install --cwd ${SRC_PATH}/common/${ARRAY_NAME_SERVICES[$i]} --frozen-lockfile && \
    mkdir -p ${BUILD_PATH}/services/${ARRAY_NAME_SERVICES[$i]}/service/ && \
    cp -rfv ${SRC_PATH}/common/${ARRAY_NAME_SERVICES[$i]}/* ${BUILD_PATH}/services/${ARRAY_NAME_SERVICES[$i]}/service/
    if [[ ${DOCKER_ENTRYPOINT} != "false" ]]
    then
       echo "== ADD ${DOCKER_ENTRYPOINT} to ${ARRAY_NAME_SERVICES[$i]} =="
       cp ${DOCKER_ENTRYPOINT} ${BUILD_PATH}/services/${ARRAY_NAME_SERVICES[$i]}/service/
    fi
  done
  ARRAY_NAME_SERVICES=()
}

# Install FRONTEND dependencies for nodjs's projects
function build_nodejs_frontend {
  echo "== yarn install =="
  yarn install
  # Install debug config mode
  if [[ $# -gt 0 ]]
  then
    local debug_info_check=$(echo $1 | tr '[:upper:]' '[:lower:]' | tr -d ' ')
    if [[ ${debug_info_check} == "true" ]]
    then
      echo "== yarn debug-info =="
	    yarn debug-info
    fi
  fi
  echo "== yarn ${FRONTEND_BUILD_ARGS} =="
  yarn ${FRONTEND_BUILD_ARGS}
  
  echo "== yarn ${FRONTEND_DEPLOY_ARGS} =="
  yarn ${FRONTEND_DEPLOY_ARGS}
  if [[ ${DOCKER_ENTRYPOINT} != "false" ]]
  then
    echo "== ADD ${DOCKER_ENTRYPOINT} to ASC.Login =="
    cp ${DOCKER_ENTRYPOINT} ${SRC_PATH}/build/deploy/login/
    echo "== ADD ${DOCKER_ENTRYPOINT} toASC.Editors =="
    cp ${DOCKER_ENTRYPOINT} ${SRC_PATH}/build/deploy/editor/
  fi
}

function run {
    case $1 in
      all )
        build_dotnetcore_backend "${MIGRATION_CHECK}"
        backend-nodejs-publish 
        build_nodejs_frontend "${DEBUG_INFO_CHECK}"
        backend-dotnet-publish
		;;
      frontend-build )
        build_nodejs_frontend "${DEBUG_INFO_CHECK}"
		;;
      backend-publish )
        build_dotnetcore_backend "${MIGRATION_CHECK}"
        backend-nodejs-publish
        backend-dotnet-publish
		;;
      
      backend-dotnet-publish )
        build_dotnetcore_backend "${MIGRATION_CHECK}"
        backend-dotnet-publish
		;;
      backend-nodejs-publish )
        backend-nodejs-publish
		;;
      backend-build )
        build_dotnetcore_backend "${MIGRATION_CHECK}"
		;;
    * )
			echo "Unknown - \"$1\", Ex. all/frontend-build/backend-publish/backend-dotnet-publish/backend-nodejs-publish/backend-build" 1>&2
			exit 1
		;;
    esac
}

run "${PROPERTY_BUILD}"
