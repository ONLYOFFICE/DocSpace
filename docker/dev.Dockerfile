ARG SRC_PATH="/app/onlyoffice/src"
ARG BUILD_PATH="/var/www"
ARG DOTNET_SDK="mcr.microsoft.com/dotnet/sdk:9.0"
ARG DOTNET_RUN="mcr.microsoft.com/dotnet/aspnet:9.0-noble"
ARG APP_STORAGE_ROOT="/app/onlyoffice/data"
ARG PATH_TO_CONF="/app/onlyoffice/config"
ARG LOG_DIR="/var/log/onlyoffice"

#----------------------------------
#         Image SDK build         
#----------------------------------
FROM $DOTNET_SDK AS build
ARG DEBIAN_FRONTEND=noninteractive
ARG ENV_EXTENSION=""
ARG VERSION=0
ARG BUILD_ARGS="build"
ARG DEPLOY_ARGS="deploy"
ARG REDIS_HOST="onlyoffice-redis"
ARG SRC_PATH
ARG BUILD_PATH
ARG APP_STORAGE_ROOT
ARG PATH_TO_CONF
ARG LOG_DIR
ARG COUNT_WORKER_CONNECTIONS=1024
ENV DNS_NAMESERVER=127.0.0.11 \
    COUNT_WORKER_CONNECTIONS=$COUNT_WORKER_CONNECTIONS \
    MAP_HASH_BUCKET_SIZE="" \
    SRC_PATH=${SRC_PATH} \
    APP_STORAGE_ROOT=${APP_STORAGE_ROOT} \
    PATH_TO_CONF=${PATH_TO_CONF} \
    LOG_DIR=${LOG_DIR} \
    BUILD_PATH=${BUILD_PATH} \
    ENV_EXTENSION=${ENV_EXTENSION} \
    REDIS_HOST=${REDIS_HOST}

#----------------------------------
#         Prepare instance         
#----------------------------------
RUN mkdir -p /var/log/onlyoffice && \
    mkdir -p /app/onlyoffice/data && \
    apt-get -y update && \
    apt-get install -yq \
        sudo \
        adduser \
        nano \
        curl \
        vim \
        gnupg \
        lsb-release \
        ca-certificates \
        gettext-base \
        supervisor && \
        curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.40.2/install.sh | bash && \
        \. "$HOME/.nvm/nvm.sh" && \
        nvm install 22 && \
        curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | gpg --dearmor -o /usr/share/keyrings/yarn-archive-keyring.gpg && \
        echo "deb [signed-by=/usr/share/keyrings/yarn-archive-keyring.gpg] https://dl.yarnpkg.com/debian/ stable main" > /etc/apt/sources.list.d/yarn.list && \
        wget -O - https://openresty.org/package/pubkey.gpg | sudo gpg --dearmor -o /usr/share/keyrings/openresty.gpg && \
        echo "deb [arch=amd64 signed-by=/usr/share/keyrings/openresty.gpg] http://openresty.org/package/debian bookworm openresty" | sudo tee /etc/apt/sources.list.d/openresty.list && \
        apt-get update && \
        apt-get install -y yarn openresty && \
        addgroup --system --gid 107 onlyoffice && \
        adduser -uid 104 --quiet --home /var/www/onlyoffice --system --gid 107 onlyoffice && \
        echo "--- clean up ---" && \
        apt-get clean && \
        rm -rf /var/lib/apt/lists/* \
        /tmp/*

#----------------------------------
#              Get src             
#----------------------------------
ADD https://api.github.com/repos/ONLYOFFICE/DocSpace-buildtools/git/refs/heads/${GIT_BRANCH} version.json
RUN git clone -b "master" --depth 1 https://github.com/ONLYOFFICE/docspace-plugins.git ${SRC_PATH}/plugins && \
    git clone -b "master" --depth 1 https://github.com/ONLYOFFICE/ASC.Web.Campaigns.git ${SRC_PATH}/campaigns
COPY --chown=onlyoffice:onlyoffice ./server/ ${SRC_PATH}/server/
COPY --chown=onlyoffice:onlyoffice ./client/ ${SRC_PATH}/client/
COPY --chown=onlyoffice:onlyoffice ./buildtools/config/ ${SRC_PATH}/buildtools/config/
COPY --chown=onlyoffice:onlyoffice ./docker/docker-entrypoint.sh /docker-entrypoint.sh
COPY --chown=onlyoffice:onlyoffice ./docker/supervisord/supervisord.conf /etc/supervisor/conf.d/supervisord.conf

#----------------------------------
#         Nodejs build             
#----------------------------------
WORKDIR ${SRC_PATH}/client

RUN <<EOF
#!/bin/bash
echo "--- build/publish docspace-client node ---" && \
yarn install
node common/scripts/before-build.js

CLIENT_PACKAGES+=("@docspace/client")
CLIENT_PACKAGES+=("@docspace/login")
CLIENT_PACKAGES+=("@docspace/doceditor")
CLIENT_PACKAGES+=("@docspace/sdk")
CLIENT_PACKAGES+=("@docspace/management")

for PKG in ${CLIENT_PACKAGES[@]}; do
  echo "--- build/publish ${PKG} ---"
  yarn workspace ${PKG} ${BUILD_ARGS} $([[ "${PKG}" =~ (client|management) ]] && echo "--env lint=false")
  yarn workspace ${PKG} ${DEPLOY_ARGS}
done

echo "--- publish public web files ---" && \
cp -rf public "${SRC_PATH}/publish/web/"
echo "--- publish locales ---" && \
node common/scripts/minify-common-locales.js
rm -rf ${SRC_PATH}/client/*
EOF

RUN echo "--- build/publish ASC.Socket.IO ---" && \ 
    cd ${SRC_PATH}/server/common/ASC.Socket.IO &&\
    yarn install --immutable && \
    mkdir -p /app/onlyoffice/src/publish/services/ASC.Socket.IO/service && \
    cp -arf ${SRC_PATH}/server/common/ASC.Socket.IO/* ${SRC_PATH}/publish/services/ASC.Socket.IO/service/ && \
    echo "--- build/publish ASC.SsoAuth ---" && \ 
    cd ${SRC_PATH}/server/common/ASC.SsoAuth && \
    yarn install --immutable && \
    mkdir -p /app/onlyoffice/src/publish/services/ASC.SsoAuth/service && \
    cp -arf ${SRC_PATH}/server/common/ASC.SsoAuth/* ${SRC_PATH}/publish/services/ASC.SsoAuth/service/

#----------------------------------
#          Dotnet build            
#----------------------------------   
WORKDIR ${SRC_PATH}/server

RUN echo "--- build/publishh docspace-server .net 9.0 ---" && \
    dotnet build ASC.Web.slnf && \
    dotnet build ASC.Migrations.sln --property:OutputPath=${SRC_PATH}/publish/services/ASC.Migration.Runner/service/ && \
    dotnet publish ASC.Web.slnf -p PublishProfile=ReleaseProfile && \
    chown onlyoffice:onlyoffice /app/onlyoffice -R && \
    chown onlyoffice:onlyoffice /var/log -R && \
    chown onlyoffice:onlyoffice /var/www -R && \
    echo "--- clean up ---" && \
    rm -rf ${SRC_PATH}/server/*

#----------------------------------
#       Nginx/Openresty            
#----------------------------------

# Create directories for openresty
RUN rm -rf /var/lib/apt/lists/* && \
    rm -rf /usr/share/nginx/html/* && \
    mkdir -p /var/log/nginx/ && \
    mkdir -p /etc/nginx/ && \
    mkdir -p /var/log/openresty && \
    mkdir -p /etc/openresty/conf.d  && \  
    mkdir -p /etc/nginx/includes/ && \
    cp -f ${SRC_PATH}/buildtools/config/nginx/includes/onlyoffice*.conf /etc/nginx/includes/ && \
    cp -f ${SRC_PATH}/buildtools/config/nginx/includes/server-*.conf /etc/nginx/includes/ && \
    mkdir -p /etc/nginx/conf.d && \
    cp -f ${SRC_PATH}/buildtools/config/nginx/onlyoffice.conf /etc/nginx/conf.d/ && \
    cp -f ${SRC_PATH}/buildtools/config/nginx/onlyoffice-client.conf /etc/nginx/conf.d/ && \
    cp -f ${SRC_PATH}/buildtools/config/nginx/onlyoffice-management.conf /etc/nginx/conf.d/ && \
    cp -f ${SRC_PATH}/buildtools/config/nginx/onlyoffice-story.conf /etc/nginx/conf.d/
    
# copy static services files and config 
RUN mkdir -p /app/onlyoffice/config && cp -arf ${SRC_PATH}/buildtools/config/* /app/onlyoffice/config/ && \
    mkdir -p ${BUILD_PATH}/client && cp -arf ${SRC_PATH}/publish/web/client/* ${BUILD_PATH}/client/ && \
    mkdir -p ${BUILD_PATH}/public && cp -arf ${SRC_PATH}/publish/web/public/* ${BUILD_PATH}/public/ && \
    mkdir -p ${BUILD_PATH}/public/campaigns && cp -arf ${SRC_PATH}/campaigns/src/campaigns/* ${BUILD_PATH}/public/campaigns/ && \
    mkdir -p ${BUILD_PATH}/management && cp -arf ${SRC_PATH}/publish/web/management/* ${BUILD_PATH}/management/ && \
    mkdir -p ${BUILD_PATH}/build/doceditor/static/chunks && cp -arf ${SRC_PATH}/publish/web/editor/.next/static/chunks/* ${BUILD_PATH}/build/doceditor/static/chunks/ && \
    mkdir -p ${BUILD_PATH}/build/doceditor/static/css && cp -arf ${SRC_PATH}/publish/web/editor/.next/static/css/* ${BUILD_PATH}/build/doceditor/static/css/ && \
    mkdir -p ${BUILD_PATH}/build/doceditor/static/media && cp -arf ${SRC_PATH}/publish/web/editor/.next/static/media/* ${BUILD_PATH}/build/doceditor/static/media/ && \
    mkdir -p ${BUILD_PATH}/build/login/static/chunks && cp -arf ${SRC_PATH}/publish/web/login/.next/static/chunks/* ${BUILD_PATH}/build/login/static/chunks/ && \
    mkdir -p ${BUILD_PATH}/build/login/static/css && cp -arf ${SRC_PATH}/publish/web/login/.next/static/css/* ${BUILD_PATH}/build/login/static/css/ && \
    mkdir -p ${BUILD_PATH}/build/login/static/media && cp -arf ${SRC_PATH}/publish/web/login/.next/static/media/* ${BUILD_PATH}/build/login/static/media/ && \
    mkdir -p ${BUILD_PATH}/build/sdk/static/chunks && cp -arf ${SRC_PATH}/publish/web/sdk/.next/static/chunks/* ${BUILD_PATH}/build/sdk/static/chunks/ && \
    mkdir -p ${BUILD_PATH}/build/sdk/static/css && cp -arf ${SRC_PATH}/publish/web/sdk/.next/static/css/* ${BUILD_PATH}/build/sdk/static/css/ && \
    mkdir -p ${BUILD_PATH}/build/sdk/static/media && cp -arf ${SRC_PATH}/publish/web/sdk/.next/static/media/* ${BUILD_PATH}/build/sdk/static/media/

COPY --chown=onlyoffice:onlyoffice ./buildtools/install/docker/config/nginx/docker-entrypoint.d /docker-entrypoint.d
COPY --chown=onlyoffice:onlyoffice ./docker/config/nginx/templates/upstream.conf.template /etc/nginx/templates/upstream.conf.template
COPY --chown=onlyoffice:onlyoffice ./buildtools/install/docker/config/nginx/templates/nginx.conf.template /etc/nginx/nginx.conf.template
COPY --chown=onlyoffice:onlyoffice ./buildtools/config/nginx/html /etc/nginx/html
COPY --chown=onlyoffice:onlyoffice ./buildtools/install/docker/prepare-nginx-router.sh /docker-entrypoint.d/prepare-nginx-router.sh
COPY --chown=onlyoffice:onlyoffice ./buildtools/install/docker/config/nginx/docker-entrypoint.sh /nginx/docker-entrypoint.sh

RUN sed -i 's/$public_root/\/var\/www\/public\//' /etc/nginx/conf.d/onlyoffice.conf && \
        sed -i 's/http:\/\/172.*/$document_server;/' /etc/nginx/conf.d/onlyoffice.conf && \
        sed -i '/client_body_temp_path/ i \ \ \ \ $MAP_HASH_BUCKET_SIZE' /etc/nginx/nginx.conf.template && \
        sed -i 's/\(worker_connections\).*;/\1 $COUNT_WORKER_CONNECTIONS;/' /etc/nginx/nginx.conf.template && \
        sed -i -e '/^user/s/^/#/' -e 's#/tmp/nginx.pid#nginx.pid#' -e 's#/etc/nginx/mime.types#mime.types#' /etc/nginx/nginx.conf.template 

RUN chown -R onlyoffice:onlyoffice /etc/nginx/ && \
    chown -R onlyoffice:onlyoffice /var/ && \
    chown -R onlyoffice:onlyoffice /usr/ && \
    chown -R onlyoffice:onlyoffice /run/ && \
    chown -R onlyoffice:onlyoffice /var/log/nginx/

WORKDIR  /   

EXPOSE 5032 5010 5027 5012 5007 5009 5005 5004 5006 5000 5003 5033 5099 5013 5011 9899 9834 8092

ENTRYPOINT  [ "/docker-entrypoint.sh" ]

## ASC.Migration.Runner ##
FROM $DOTNET_RUN AS onlyoffice-migration-runner
ARG BUILD_PATH
ARG SRC_PATH
ENV BUILD_PATH=${BUILD_PATH} \
    SRC_PATH=${SRC_PATH}
WORKDIR ${BUILD_PATH}/services/ASC.Migration.Runner/

COPY --chown=onlyoffice:onlyoffice ./buildtools/install/docker/docker-migration-entrypoint.sh ./docker-migration-entrypoint.sh
COPY --from=build --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/services/ASC.Migration.Runner/service/ .

ENTRYPOINT ["./docker-migration-entrypoint.sh"]
