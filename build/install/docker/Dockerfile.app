ARG SRC_PATH="/app/onlyoffice/src"
ARG BUILD_PATH="/var/www"
ARG DOTNET_SDK="mcr.microsoft.com/dotnet/sdk:6.0"
ARG DOTNET_RUN="mcr.microsoft.com/dotnet/aspnet:6.0"

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

LABEL onlyoffice.appserver.release-date="${RELEASE_DATE}" \
      maintainer="Ascensio System SIA <support@onlyoffice.com>"

ENV LANG=en_US.UTF-8 \
    LANGUAGE=en_US:en \
    LC_ALL=en_US.UTF-8

RUN apt-get -y update && \
    apt-get -y upgrade && \
    apt-get -y dist-upgrade && \
    apt-get install -yq sudo locales && \
    addgroup --system --gid 107 onlyoffice && \
    adduser -uid 104 --quiet --home /var/www/onlyoffice --system --gid 107 onlyoffice && \
    locale-gen en_US.UTF-8 && \
    apt-get -y update && \
    apt-get install -yq git apt-utils npm && \
    npm install --global yarn && \
    curl -fsSL https://deb.nodesource.com/setup_16.x | sudo -E bash - && \
    apt-get install -y nodejs

RUN echo ${GIT_BRANCH}  && \
    git clone --recurse-submodules -b ${GIT_BRANCH} https://github.com/ONLYOFFICE/AppServer.git ${SRC_PATH}

RUN cd ${SRC_PATH} && \
    # mkdir -p /app/onlyoffice/config/ && cp -rf config/* /app/onlyoffice/config/ && \
    mkdir -p /app/onlyoffice/ && \
    find config/ -maxdepth 1 -name "*.json" | grep -v test | xargs tar -cvf config.tar && \
    tar -C "/app/onlyoffice/" -xvf config.tar && \
    cp config/*.config /app/onlyoffice/config/ && \
    mkdir -p /etc/nginx/conf.d && cp -f config/nginx/onlyoffice*.conf /etc/nginx/conf.d/ && \
    mkdir -p /etc/nginx/includes/ && cp -f config/nginx/includes/onlyoffice*.conf /etc/nginx/includes/ && \
    sed -i "s/\"number\".*,/\"number\": \"${PRODUCT_VERSION}.${BUILD_NUMBER}\",/g" /app/onlyoffice/config/appsettings.json && \
    sed -e 's/#//' -i /etc/nginx/conf.d/onlyoffice.conf && \
    cd ${SRC_PATH}/build/install/common/ && \
    bash build-frontend.sh -sp "${SRC_PATH}" -ba "${BUILD_ARGS}" -da "${DEPLOY_ARGS}" -di "${DEBUG_INFO}" && \
    bash build-backend.sh -sp "${SRC_PATH}"  && \
    bash publish-backend.sh -sp "${SRC_PATH}" -bp "${BUILD_PATH}"  && \
    cp -rf ${SRC_PATH}/products/ASC.Files/Server/DocStore ${BUILD_PATH}/products/ASC.Files/server/ && \
    rm -rf ${SRC_PATH}/common/* && \
    rm -rf ${SRC_PATH}/web/ASC.Web.Core/* && \
    rm -rf ${SRC_PATH}/web/ASC.Web.Studio/* && \
    rm -rf ${SRC_PATH}/products/ASC.Files/Server/* && \
    rm -rf ${SRC_PATH}/products/ASC.Files/Service/* && \
    rm -rf ${SRC_PATH}/products/ASC.People/Server/* 
  
COPY config/mysql/conf.d/mysql.cnf /etc/mysql/conf.d/mysql.cnf

RUN rm -rf /var/lib/apt/lists/*

FROM $DOTNET_RUN as builder
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
    apt-get -y upgrade && \
    apt-get install -yq sudo nano curl vim python3-pip && \
    apt-get install -yq libgdiplus && \
    pip3 install --upgrade jsonpath-ng multipledispatch

COPY --from=base --chown=onlyoffice:onlyoffice /app/onlyoffice/config/* /app/onlyoffice/config/
        
#USER onlyoffice
EXPOSE 5050
ENTRYPOINT ["python3", "docker-entrypoint.py"]

FROM node:16.16-slim as nodeBuild
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
    apt-get -y upgrade && \
    apt-get install -yq sudo nano curl vim python3-pip && \
    pip3 install --upgrade jsonpath-ng multipledispatch

COPY --from=base --chown=onlyoffice:onlyoffice /app/onlyoffice/config/* /app/onlyoffice/config/

EXPOSE 5050
ENTRYPOINT ["python3", "docker-entrypoint.py"]

## Nginx image ##
FROM nginx AS web
ARG SRC_PATH
ARG BUILD_PATH
ARG COUNT_WORKER_CONNECTIONS=1024
ENV DNS_NAMESERVER=127.0.0.11 \
    COUNT_WORKER_CONNECTIONS=$COUNT_WORKER_CONNECTIONS \
    MAP_HASH_BUCKET_SIZE=""

RUN apt-get -y update && \
    apt-get -y upgrade && \
    apt-get install -yq vim && \
    # Remove default nginx website
    rm -rf /usr/share/nginx/html/* 

# copy static services files and config values 
COPY --from=base /etc/nginx/conf.d /etc/nginx/conf.d
COPY --from=base /etc/nginx/includes /etc/nginx/includes
COPY --from=base ${SRC_PATH}/build/deploy/client ${BUILD_PATH}/client
COPY --from=base ${SRC_PATH}/build/deploy/login ${BUILD_PATH}/login
COPY --from=base ${SRC_PATH}/build/deploy/public ${BUILD_PATH}/public
COPY /config/nginx/templates/upstream.conf.template /etc/nginx/templates/upstream.conf.template
COPY /config/nginx/templates/nginx.conf.template /etc/nginx/nginx.conf.template
COPY prepare-nginx-proxy.sh /docker-entrypoint.d/prepare-nginx-proxy.sh

# add defualt user and group for no-root run
RUN chown nginx:nginx /etc/nginx/* -R && \
    chown nginx:nginx /docker-entrypoint.d/* && \
    # changes for upstream configure
    sed -i 's/localhost:5010/$service_api_system/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/localhost:5012/$service_backup/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/localhost:5021/$service_crm/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/localhost:5007/$service_files/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/localhost:5004/$service_people_server/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/localhost:5020/$service_projects_server/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/localhost:5000/$service_api/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/localhost:5003/$service_studio/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/localhost:5023/$service_calendar/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/localhost:9899/$service_socket/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/localhost:9834/$service_sso/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/localhost:5022/$service_mail/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/localhost:9999/$service_urlshortener/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/localhost:5034/$service_migration/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/localhost:5013/$service_doceditor/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/172.*/$document_server;/' /etc/nginx/conf.d/onlyoffice.conf

## Doceditor ##
FROM nodeBuild as doceditor
WORKDIR ${BUILD_PATH}/products/ASC.Files/editor

COPY --from=base --chown=onlyoffice:onlyoffice ${SRC_PATH}/build/deploy/editor/ .
EXPOSE 5013
ENTRYPOINT ["node", "server.js"]

## ASC.Data.Backup.BackgroundTasks ##
FROM builder AS backup_background
WORKDIR ${BUILD_PATH}/services/ASC.Data.Backup.BackgroundTasks/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Data.Backup.BackgroundTasks/service/  .

CMD ["ASC.Data.Backup.BackgroundTasks.dll", "ASC.Data.Backup.BackgroundTasks"]

## ASC.ClearEvents ##
FROM builder AS clear-events
WORKDIR ${BUILD_PATH}/services/ASC.ClearEvents/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.ClearEvents/service/  .

CMD ["ASC.ClearEvents.dll", "ASC.ClearEvents"]

## ASC.Migration ##
FROM builder AS migration
WORKDIR ${BUILD_PATH}/services/ASC.Migration/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Migration/service/  .

CMD ["ASC.Migration.dll", "ASC.Migration"]

## ASC.Data.Backup ##
FROM builder AS backup
WORKDIR ${BUILD_PATH}/services/ASC.Data.Backup/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Data.Backup/service/ .

CMD ["ASC.Data.Backup.dll", "ASC.Data.Backup"]

## ASC.Files ##
FROM builder AS files
WORKDIR ${BUILD_PATH}/products/ASC.Files/server/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/products/ASC.Files/server/ .

CMD ["ASC.Files.dll", "ASC.Files"]

## ASC.Files.Service ##
FROM builder AS files_services
WORKDIR ${BUILD_PATH}/products/ASC.Files/service/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Files.Service/service/ .

CMD ["ASC.Files.Service.dll", "ASC.Files.Service"]

## ASC.Notify ##
FROM builder AS notify
WORKDIR ${BUILD_PATH}/services/ASC.Notify/service

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Notify/service/ .

CMD ["ASC.Notify.dll", "ASC.Notify"]

## ASC.People ##
FROM builder AS people_server
WORKDIR ${BUILD_PATH}/products/ASC.People/server/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/products/ASC.People/server/ .

CMD ["ASC.People.dll", "ASC.People"]

## ASC.Socket.IO ##
FROM nodeBuild AS socket
WORKDIR ${BUILD_PATH}/services/ASC.Socket.IO/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Socket.IO/service/ .

CMD  ["server.js", "ASC.Socket.IO"]

## ASC.SsoAuth ##
FROM nodeBuild AS ssoauth
WORKDIR ${BUILD_PATH}/services/ASC.SsoAuth/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice  ${BUILD_PATH}/services/ASC.SsoAuth/service/ .

CMD ["app.js", "ASC.SsoAuth"]

## ASC.Studio.Notify ##
FROM builder AS studio_notify
WORKDIR ${BUILD_PATH}/services/ASC.Studio.Notify/service/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Studio.Notify/service/ .

CMD ["ASC.Studio.Notify.dll", "ASC.Studio.Notify"]

## ASC.TelegramService ##
FROM builder AS telegram_service
WORKDIR ${BUILD_PATH}/services/ASC.TelegramService/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.TelegramService/service/ .

CMD ["ASC.TelegramService.dll", "ASC.TelegramService"]

## ASC.UrlShortener ##
FROM nodeBuild AS urlshortener
WORKDIR  ${BUILD_PATH}/services/ASC.UrlShortener/service/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice  ${BUILD_PATH}/services/ASC.UrlShortener/service/ .

CMD ["index.js", "ASC.UrlShortener"]

## ASC.Web.Api ##
FROM builder AS api
WORKDIR ${BUILD_PATH}/studio/ASC.Web.Api/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Web.Api/service/ .

CMD ["ASC.Web.Api.dll", "ASC.Web.Api"]

## ASC.Webhooks.Service ##
FROM builder AS webhooks-service
WORKDIR ${BUILD_PATH}/services/ASC.Webhooks.Service/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Webhooks.Service/service/  .

CMD ["ASC.Webhooks.Service.dll", "ASC.Webhooks.Service"]

## ASC.Web.Studio ##
FROM builder AS studio
WORKDIR ${BUILD_PATH}/studio/ASC.Web.Studio/

COPY --chown=onlyoffice:onlyoffice docker-entrypoint.py ./docker-entrypoint.py
COPY --from=base --chown=onlyoffice:onlyoffice ${BUILD_PATH}/services/ASC.Web.Studio/service/ .

CMD ["ASC.Web.Studio.dll", "ASC.Web.Studio"]

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
