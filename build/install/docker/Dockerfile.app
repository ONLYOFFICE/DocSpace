ARG SRC_PATH="/app/onlyoffice/src"
ARG BUILD_PATH="/var/www"
ARG DOTNET_SDK="mcr.microsoft.com/dotnet/sdk:7.0"
ARG DOTNET_RUN="mcr.microsoft.com/dotnet/aspnet:7.0"

FROM $DOTNET_SDK AS base
ARG RELEASE_DATE="2016-06-22"
ARG DEBIAN_FRONTEND=noninteractive
ARG PRODUCT_VERSION=0.0.0
ARG BUILD_NUMBER=0
ARG GIT_BRANCH="master"
ARG SRC_PATH
ARG BUILD_PATH
ARG BUILD_ARGS="build"
ARG DEPLOY_ARGS="deploy"
ARG DEBUG_INFO="true"
ARG PUBLISH_CNF="Release"

LABEL onlyoffice.appserver.release-date="${RELEASE_DATE}" \
      maintainer="Ascensio System SIA <support@onlyoffice.com>"

ENV LANG=en_US.UTF-8 \
    LANGUAGE=en_US:en \
    LC_ALL=en_US.UTF-8

RUN apt-get -y update && \
    apt-get install -yq \
        sudo \
        locales \
        git \
        npm  && \
    locale-gen en_US.UTF-8 && \
    npm install --global yarn && \
    curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash - && \
    apt-get install -y nodejs && \
    rm -rf /var/lib/apt/lists/*

ADD https://api.github.com/repos/lemmav/DocSpace/git/refs/heads/${GIT_BRANCH} version.json
RUN echo ${GIT_BRANCH}  && \
    git clone --recurse-submodules -b ${GIT_BRANCH} https://github.com/lemmav/DocSpace.git ${SRC_PATH}

RUN cd ${SRC_PATH} && \
    # mkdir -p /app/onlyoffice/config/ && cp -rf config/* /app/onlyoffice/config/ && \
    mkdir -p /app/onlyoffice/ && \
    find config/ -maxdepth 1 -name "*.json" | grep -v test | grep -v dev | xargs tar -cvf config.tar && \
    tar -C "/app/onlyoffice/" -xvf config.tar && \
    cp config/*.config /app/onlyoffice/config/ && \
    mkdir -p /etc/nginx/conf.d && cp -f config/nginx/onlyoffice*.conf /etc/nginx/conf.d/ && \
    mkdir -p /etc/nginx/includes/ && cp -f config/nginx/includes/onlyoffice*.conf /etc/nginx/includes/ && \
    sed -i "s/\"number\".*,/\"number\": \"${PRODUCT_VERSION}.${BUILD_NUMBER}\",/g" /app/onlyoffice/config/appsettings.json && \
    sed -e 's/#//' -i /etc/nginx/conf.d/onlyoffice.conf && \
    cd ${SRC_PATH}/build/install/common/ && \
    bash build-frontend.sh -sp "${SRC_PATH}" -ba "${BUILD_ARGS}" -da "${DEPLOY_ARGS}" -di "${DEBUG_INFO}" && \
    bash build-backend.sh -sp "${SRC_PATH}"  && \
    bash publish-backend.sh -pc "${PUBLISH_CNF}" -sp "${SRC_PATH}" -bp "${BUILD_PATH}"  && \
    cp -rf ${SRC_PATH}/products/ASC.Files/Server/DocStore ${BUILD_PATH}/products/ASC.Files/server/ && \
    rm -rf ${SRC_PATH}/common/* && \
    rm -rf ${SRC_PATH}/web/ASC.Web.Core/* && \
    rm -rf ${SRC_PATH}/web/ASC.Web.Studio/* && \
    rm -rf ${SRC_PATH}/products/ASC.Files/Server/* && \
    rm -rf ${SRC_PATH}/products/ASC.Files/Service/* && \
    rm -rf ${SRC_PATH}/products/ASC.People/Server/* 
  
COPY config/mysql/conf.d/mysql.cnf /etc/mysql/conf.d/mysql.cnf

FROM $DOTNET_RUN as dotnetrun
ARG BUILD_PATH
ARG SRC_PATH
ENV BUILD_PATH=${BUILD_PATH}
ENV SRC_PATH=${SRC_PATH}

# add defualt user and group for no-root run
RUN mkdir -p /var/log/onlyoffice && \
    mkdir -p /app/onlyoffice/data && \
    addgroup --system --gid 107 onlyoffice && \
    adduser -uid 104 --quiet --home /var/www/onlyoffice --system --gid 107 onlyoffice && \
    chown onlyoffice:onlyoffice /app/onlyoffice -R && \
    chown onlyoffice:onlyoffice /var/log -R && \
    chown onlyoffice:onlyoffice /var/www -R && \
    apt-get -y update && \
    apt-get install -yq \
        sudo \
        nano \
        curl \
        vim \
        python3-pip \
        libgdiplus && \
    pip3 install --upgrade jsonpath-ng multipledispatch netaddr netifaces && \
    rm -rf /var/lib/apt/lists/*

COPY --from=base --chown=onlyoffice:onlyoffice /app/onlyoffice/config/* /app/onlyoffice/config/
        
#USER onlyoffice
EXPOSE 5050
ENTRYPOINT ["python3", "docker-entrypoint.py"]

FROM node:18.12.1-slim as noderun
ARG BUILD_PATH
ARG SRC_PATH 
ENV BUILD_PATH=${BUILD_PATH}
ENV SRC_PATH=${SRC_PATH}

RUN mkdir -p /var/log/onlyoffice && \
    mkdir -p /app/onlyoffice/data && \
    addgroup --system --gid 107 onlyoffice && \
    adduser -uid 104 --quiet --home /var/www/onlyoffice --system --gid 107 onlyoffice && \
    chown onlyoffice:onlyoffice /app/onlyoffice -R && \
    chown onlyoffice:onlyoffice /var/log -R  && \
    chown onlyoffice:onlyoffice /var/www -R && \
    apt-get -y update && \
    apt-get install -yq \ 
        sudo \
        nano \
        curl \
        vim \
        python3-pip && \
    pip3 install --upgrade jsonpath-ng multipledispatch netaddr netifaces && \
    rm -rf /var/lib/apt/lists/*

COPY --from=base --chown=onlyoffice:onlyoffice /app/onlyoffice/config/* /app/onlyoffice/config/

EXPOSE 5050
ENTRYPOINT ["python3", "docker-entrypoint.py"]

## Nginx image ##
FROM nginx AS proxy
ARG SRC_PATH
ARG BUILD_PATH
ARG COUNT_WORKER_CONNECTIONS=1024
ENV DNS_NAMESERVER=127.0.0.11 \
    COUNT_WORKER_CONNECTIONS=$COUNT_WORKER_CONNECTIONS \
    MAP_HASH_BUCKET_SIZE=""

RUN apt-get -y update && \
    apt-get install -yq vim && \
    rm -rf /var/lib/apt/lists/* && \
    rm -rf /usr/share/nginx/html/* 

# copy static services files and config values 
COPY --from=base /etc/nginx/conf.d /etc/nginx/conf.d
COPY --from=base /etc/nginx/includes /etc/nginx/includes
COPY --from=base ${SRC_PATH}/build/deploy/client ${BUILD_PATH}/client
COPY --from=base ${SRC_PATH}/build/deploy/public ${BUILD_PATH}/public
COPY /config/nginx/templates/upstream.conf.template /etc/nginx/templates/upstream.conf.template
COPY /config/nginx/templates/nginx.conf.template /etc/nginx/nginx.conf.template
COPY prepare-nginx-proxy.sh /docker-entrypoint.d/prepare-nginx-proxy.sh

# add defualt user and group for no-root run
RUN chown nginx:nginx /etc/nginx/* -R && \
    chown nginx:nginx /docker-entrypoint.d/* && \
    # changes for upstream configure
    sed -i 's/127.0.0.1:5010/$service_api_system/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/127.0.0.1:5012/$service_backup/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/127.0.0.1:5007/$service_files/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/127.0.0.1:5004/$service_people_server/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/127.0.0.1:5000/$service_api/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/127.0.0.1:5003/$service_studio/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/127.0.0.1:9899/$service_socket/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/127.0.0.1:9834/$service_sso/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/127.0.0.1:5013/$service_doceditor/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/127.0.0.1:5011/$service_login/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/127.0.0.1:5033/$service_healthchecks/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/$public_root/\/var\/www\/public\//' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/172.*/$document_server;/' /etc/nginx/conf.d/onlyoffice.conf

## Doceditor ##
FROM noderun as doceditor
WORKDIR ${BUILD_PATH}/products/ASC.Editors/editor

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${SRC_PATH}/build/deploy/editor/ .

CMD ["server.js", "ASC.Editors"]

## Login ##
FROM noderun as login
WORKDIR ${BUILD_PATH}/products/ASC.Login/login

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${SRC_PATH}/build/deploy/login/ .

CMD ["server.js", "ASC.Login"]

## ASC.Data.Backup.BackgroundTasks ##
FROM dotnetrun AS backup_background
WORKDIR ${BUILD_PATH}/services/ASC.Data.Backup.BackgroundTasks/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Data.Backup.BackgroundTasks/service/  .

CMD ["ASC.Data.Backup.BackgroundTasks.dll", "ASC.Data.Backup.BackgroundTasks", "core:eventBus:subscriptionClientName=asc_event_bus_backup_queue"]

# ASC.ApiSystem ##
FROM dotnetrun AS api_system
WORKDIR ${BUILD_PATH}/services/ASC.ApiSystem/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.ApiSystem/service/  .

CMD ["ASC.ApiSystem.dll", "ASC.ApiSystem"]

## ASC.ClearEvents ##
FROM dotnetrun AS clear-events
WORKDIR ${BUILD_PATH}/services/ASC.ClearEvents/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.ClearEvents/service/  .

CMD ["ASC.ClearEvents.dll", "ASC.ClearEvents"]

## ASC.Data.Backup ##
FROM dotnetrun AS backup
WORKDIR ${BUILD_PATH}/services/ASC.Data.Backup/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Data.Backup/service/ .

CMD ["ASC.Data.Backup.dll", "ASC.Data.Backup"]

## ASC.Files ##
FROM dotnetrun AS files
WORKDIR ${BUILD_PATH}/products/ASC.Files/server/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/products/ASC.Files/server/ .

CMD ["ASC.Files.dll", "ASC.Files"]

## ASC.Files.Service ##
FROM dotnetrun AS files_services
ENV LD_LIBRARY_PATH=/usr/local/lib:/usr/local/lib64
WORKDIR ${BUILD_PATH}/products/ASC.Files/service/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Files.Service/service/ .
COPY --from=onlyoffice/ffvideo:6.0 --chown=onlyoffice:onlyoffice /usr/local /usr/local/

CMD ["ASC.Files.Service.dll", "ASC.Files.Service", "core:eventBus:subscriptionClientName=asc_event_bus_files_service_queue"]

## ASC.Notify ##
FROM dotnetrun AS notify
WORKDIR ${BUILD_PATH}/services/ASC.Notify/service

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Notify/service/ .

CMD ["ASC.Notify.dll", "ASC.Notify", "core:eventBus:subscriptionClientName=asc_event_bus_notify_queue"]

## ASC.People ##
FROM dotnetrun AS people_server
WORKDIR ${BUILD_PATH}/products/ASC.People/server/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/products/ASC.People/server/ .

CMD ["ASC.People.dll", "ASC.People"]

## ASC.Socket.IO ##
FROM noderun AS socket
WORKDIR ${BUILD_PATH}/services/ASC.Socket.IO/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Socket.IO/service/ .

CMD  ["server.js", "ASC.Socket.IO"]

## ASC.SsoAuth ##
FROM noderun AS ssoauth
WORKDIR ${BUILD_PATH}/services/ASC.SsoAuth/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice  ${BUILD_PATH}/services/ASC.SsoAuth/service/ .

CMD ["app.js", "ASC.SsoAuth"]

## ASC.Studio.Notify ##
FROM dotnetrun AS studio_notify
WORKDIR ${BUILD_PATH}/services/ASC.Studio.Notify/service/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Studio.Notify/service/ .

CMD ["ASC.Studio.Notify.dll", "ASC.Studio.Notify"]

## ASC.Web.Api ##
FROM dotnetrun AS api
WORKDIR ${BUILD_PATH}/studio/ASC.Web.Api/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Web.Api/service/ .

CMD ["ASC.Web.Api.dll", "ASC.Web.Api"]

## ASC.Web.Studio ##
FROM dotnetrun AS studio
WORKDIR ${BUILD_PATH}/studio/ASC.Web.Studio/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Web.Studio/service/ .

CMD ["ASC.Web.Studio.dll", "ASC.Web.Studio"]

## ASC.Web.HealthChecks.UI ##
FROM dotnetrun AS healthchecks
WORKDIR ${BUILD_PATH}/services/ASC.Web.HealthChecks.UI/service

COPY --chown=onlyoffice:onlyoffice docker-healthchecks-entrypoint.sh ./docker-healthchecks-entrypoint.sh
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Web.HealthChecks.UI/service/ .

ENTRYPOINT ["./docker-healthchecks-entrypoint.sh"]
CMD ["ASC.Web.HealthChecks.UI.dll", "ASC.Web.HealthChecks.UI"]

## ASC.Migration.Runner ##
FROM $DOTNET_RUN AS onlyoffice-migration-runner
ARG BUILD_PATH
ARG SRC_PATH 
ENV BUILD_PATH=${BUILD_PATH}
ENV SRC_PATH=${SRC_PATH}
WORKDIR ${BUILD_PATH}/services/ASC.Migration.Runner/
COPY  ./docker-migration-entrypoint.sh ./docker-migration-entrypoint.sh
COPY --from=base ${SRC_PATH}/ASC.Migration.Runner/service/ .

ENTRYPOINT ["./docker-migration-entrypoint.sh"]

## image for k8s bin-share ##
FROM busybox:latest AS bin_share
RUN mkdir -p /app/appserver/ASC.Files/server && \
    mkdir -p /app/appserver/ASC.People/server/ && \
    mkdir -p /app/appserver/ASC.CRM/server/ && \
    mkdir -p /app/appserver/ASC.Projects/server/ && \
    mkdir -p /app/appserver/ASC.Calendar/server/ && \
    mkdir -p /app/appserver/ASC.Mail/server/ && \
    addgroup --system --gid 107 onlyoffice && \
    adduser -u 104 onlyoffice --home /var/www/onlyoffice --system -G onlyoffice

COPY bin-share-docker-entrypoint.sh /app/docker-entrypoint.sh
COPY --from=base /var/www/products/ASC.Files/server/ /app/appserver/ASC.Files/server/
COPY --from=base /var/www/products/ASC.People/server/ /app/appserver/ASC.People/server/
ENTRYPOINT ["./app/docker-entrypoint.sh"]

## image for k8s wait-bin-share ##
FROM busybox:latest AS wait_bin_share
RUN mkdir /app

COPY wait-bin-share-docker-entrypoint.sh /app/docker-entrypoint.sh
ENTRYPOINT ["./app/docker-entrypoint.sh"]
