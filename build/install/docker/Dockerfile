FROM ubuntu:18.04

ARG RELEASE_DATE="2016-06-21"
ARG RELEASE_DATE_SIGN=""
ARG VERSION="8.9.0.190"
ARG SOURCE_REPO_URL="deb http://static.teamlab.com.s3.amazonaws.com/repo/debian squeeze main"
ARG DEBIAN_FRONTEND=noninteractive
ARG GIT_BRANCH="develop"

LABEL onlyoffice.community.release-date="${RELEASE_DATE}" \
      onlyoffice.community.version="${VERSION}" \
      maintainer="Ascensio System SIA <support@onlyoffice.com>"

ENV LANG=en_US.UTF-8 \
    LANGUAGE=en_US:en \
    LC_ALL=en_US.UTF-8

RUN apt-get -y update && \
    apt-get -y upgrade && \
    apt-get -y dist-upgrade && \
    apt-get -yq install gnupg2 ca-certificates && \
    apt-get install -yq sudo locales && \
    addgroup --system --gid 107 onlyoffice && \
    adduser -uid 104 --quiet --home /var/www/onlyoffice --system --gid 107 onlyoffice && \
    addgroup --system --gid 104 elasticsearch && \
    adduser -uid 103 --quiet --home /nonexistent --system --gid 104 elasticsearch && \
    locale-gen en_US.UTF-8 && \
    apt-get -y update && \
    apt-get install -yq software-properties-common wget curl cron rsyslog && \
    curl -OL http://dev.mysql.com/get/mysql-apt-config_0.8.15-1_all.deb && \
    echo "mysql-apt-config mysql-apt-config/repo-codename select bionic" | sudo debconf-set-selections && \
    echo "mysql-apt-config mysql-apt-config/repo-distro select ubuntu" | sudo debconf-set-selections && \
    echo "mysql-apt-config mysql-apt-config/select-server select mysql-8.0" | sudo debconf-set-selections && \
    DEBIAN_FRONTEND=noninteractive dpkg -i mysql-apt-config_0.8.15-1_all.deb && \
    rm -f mysql-apt-config_0.8.15-1_all.deb && \
    wget http://nginx.org/keys/nginx_signing.key && \
    apt-key add nginx_signing.key && \
    echo "deb http://nginx.org/packages/mainline/ubuntu/ bionic nginx" >> /etc/apt/sources.list.d/nginx.list && \
    echo "deb-src http://nginx.org/packages/mainline/ubuntu/ bionic nginx" >> /etc/apt/sources.list.d/nginx.list && \
    apt-get install -yq openjdk-8-jre-headless && \
    wget -qO - https://artifacts.elastic.co/GPG-KEY-elasticsearch | apt-key add - && \
    apt-get install -yq apt-transport-https && \
    apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF && \
    echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" | tee /etc/apt/sources.list.d/mono-official.list && \
    echo "deb https://artifacts.elastic.co/packages/7.x/apt stable main" | tee -a /etc/apt/sources.list.d/elastic-7.x.list && \
    apt-get -y update && \
    apt-get install -yq elasticsearch=7.4.0 && \
    add-apt-repository -y ppa:certbot/certbot && \
    curl -sL https://deb.nodesource.com/setup_12.x | sudo -E bash - && \
    apt-get install -y nodejs && \
    curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | sudo apt-key add - && \
    echo "deb https://dl.yarnpkg.com/debian/ stable main" | sudo tee /etc/apt/sources.list.d/yarn.list && \
    wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    apt-get -y update && \
    apt-get install -yq nginx && \
    cd ~ && \
    wget https://downloads.apache.org/kafka/2.5.0/kafka_2.12-2.5.0.tgz && \
    tar xzf kafka_2.12-2.5.0.tgz && \
    rm kafka_2.12-2.5.0.tgz && \
    echo "#!/bin/sh\nexit 0" > /usr/sbin/policy-rc.d && \
    apt-get install -yq libgdiplus \
                        python-certbot-nginx \
                        htop \
                        nano \
                        dnsutils \
                        python3-pip \
                        multiarch-support \ 
                        iproute2 \
                        ffmpeg \
                        jq \ 
                        git \ 
                        yarn \
                        dotnet-sdk-3.1 \
                        supervisor \
                        mysql-client \
                        mysql-server

RUN git clone https://github.com/ONLYOFFICE/AppServer.git /app/onlyoffice/src/ && \
    cd /app/onlyoffice/src/ && \
    git checkout ${GIT_BRANCH} && \
    git pull

RUN cd /app/onlyoffice/src/ && \
    yarn install --cwd web/ASC.Web.Components --frozen-lockfile > build/ASC.Web.Components.log && \
    yarn pack --cwd web/ASC.Web.Components
	
RUN cd /app/onlyoffice/src/ && \
    component=$(ls web/ASC.Web.Components/asc-web-components-v1.*.tgz) && \
    yarn remove asc-web-components --cwd web/ASC.Web.Common --peer && \
    yarn add file:../../$component --cwd web/ASC.Web.Common --cache-folder ../../yarn --peer && \
    yarn install --cwd web/ASC.Web.Common --frozen-lockfile > build/ASC.Web.Common.log && \
    yarn pack --cwd web/ASC.Web.Common

RUN cd /app/onlyoffice/src/ && \
    npm run build:storybook --prefix web/ASC.Web.Components && \
    mkdir -p /var/www/story/ && \
    cp -Rf web/ASC.Web.Components/storybook-static/* /var/www/story/

RUN cd /app/onlyoffice/src/ && \
    component=$(ls web/ASC.Web.Components/asc-web-components-v1.*.tgz) && \
    common=$(ls web/ASC.Web.Common/asc-web-common-v1.*.tgz) && \
    yarn remove asc-web-components asc-web-common --cwd web/ASC.Web.Client && \
    yarn add ../../$component --cwd web/ASC.Web.Client --cache-folder ../../yarn && \
    yarn add ../../$common --cwd web/ASC.Web.Client --cache-folder ../../yarn && \
    yarn install --cwd web/ASC.Web.Client --frozen-lockfile || (cd web/ASC.Web.Client && npm i && cd ../../) && \
    npm run build --prefix web/ASC.Web.Client && \
    rm -rf /var/www/studio/client/* && \
    mkdir -p /var/www/studio/client && \
    cp -rf web/ASC.Web.Client/build/* /var/www/studio/client

RUN cd /app/onlyoffice/src/ && \
    component=$(ls  web/ASC.Web.Components/asc-web-components-v1.*.tgz) && \
    common=$(ls web/ASC.Web.Common/asc-web-common-v1.*.tgz) && \
    yarn remove asc-web-components asc-web-common --cwd products/ASC.Files/Client && \
    yarn add ../../../$component --cwd products/ASC.Files/Client --cache-folder ../../../yarn && \
    yarn add ../../../$common --cwd products/ASC.Files/Client --cache-folder ../../../yarn && \
    yarn install --cwd products/ASC.Files/Client --frozen-lockfile || (cd products/ASC.Files/Client && npm i && cd ../../../) && \
    npm run build --prefix products/ASC.Files/Client && \
    mkdir -p /var/www/products/ASC.Files/client && \
    cp -Rf products/ASC.Files/Client/build/* /var/www/products/ASC.Files/client && \
    mkdir -p /var/www/products/ASC.Files/client/products/files

RUN cd /app/onlyoffice/src/ && \
    component=$(ls  web/ASC.Web.Components/asc-web-components-v1.*.tgz) && \
    common=$(ls web/ASC.Web.Common/asc-web-common-v1.*.tgz) && \
    yarn remove asc-web-components asc-web-common --cwd products/ASC.People/Client && \
    yarn add ../../../$component --cwd products/ASC.People/Client --cache-folder ../../../yarn && \
    yarn add ../../../$common --cwd products/ASC.People/Client --cache-folder ../../../yarn && \
    yarn install --cwd products/ASC.People/Client --frozen-lockfile || (cd products/ASC.People/Client && npm i && cd ../../../) && \
    npm run build --prefix products/ASC.People/Client && \
    mkdir -p /var/www/products/ASC.People/client && \
    cp -Rf products/ASC.People/Client/build/* /var/www/products/ASC.People/client && \
    mkdir -p /var/www/products/ASC.People/client/products/people

RUN cd /app/onlyoffice/src/ && \
    rm -f /etc/nginx/conf.d/* && \
    mkdir -p /var/www/public/ && cp -f public/* /var/www/public/ && \
    mkdir -p /app/onlyoffice/config/ && cp -rf config/* /app/onlyoffice/config/ && \
    cp -f config/nginx/onlyoffice*.conf /etc/nginx/conf.d/ && \
    mkdir -p /etc/nginx/includes/ && cp -f config/nginx/includes/onlyoffice*.conf /etc/nginx/includes/ && \
    sed -e 's/#//' -i /etc/nginx/conf.d/onlyoffice.conf && \
    dotnet restore ASC.Web.sln && \
    dotnet build -r linux-x64 ASC.Web.sln && \
    cd products/ASC.People/Server && \
    dotnet -d publish --no-build --self-contained -r linux-x64 -o /var/www/products/ASC.People/server && \
    cd ../../../ && \
    cd products/ASC.Files/Server && \
    dotnet -d publish --no-build --self-contained -r linux-x64 -o /var/www/products/ASC.Files/server && \
    cp -avrf DocStore /var/www/products/ASC.Files/server/ && \
    cd ../../../ && \
    cd products/ASC.Files/Service && \
    dotnet -d publish --no-build --self-contained -r linux-x64 -o /var/www/products/ASC.Files/service && \
    cd ../../../ && \
    cd web/ASC.Web.Api && \
    dotnet -d publish --no-build --self-contained -r linux-x64 -o /var/www/studio/api && \
    cd ../../ && \
    cd web/ASC.Web.Studio && \
    dotnet -d publish --no-build --self-contained -r linux-x64 -o /var/www/studio/server && \
    cd ../../ && \
    cd common/services/ASC.Data.Backup && \
    dotnet -d publish --no-build --self-contained -r linux-x64 -o /var/www/services/backup && \
    cd ../../../ && \
    cd common/services/ASC.Notify && \
    dotnet -d publish --no-build --self-contained -r linux-x64 -o /var/www/services/notify && \
    cd ../../../ && \
    cd common/services/ASC.ApiSystem && \
    dotnet -d publish --no-build --self-contained -r linux-x64 -o /var/www/services/apisystem && \
    cd ../../../ && \
    cd common/services/ASC.Thumbnails.Svc && \
    dotnet -d publish --no-build --self-contained -r linux-x64 -o /services/thumb/service && \
    cd ../../../ && \
    yarn install --cwd common/ASC.Thumbnails --frozen-lockfile && \
    mkdir -p /var/www/services/thumb/client && cp -Rf common/ASC.Thumbnails/* /var/www/services/thumb/client && \
    cd common/services/ASC.UrlShortener.Svc && \
    dotnet -d publish --no-build --self-contained -r linux-x64 -o /services/urlshortener/service && \
    cd ../../../ && \
    yarn install --cwd common/ASC.UrlShortener --frozen-lockfile && \
    mkdir -p /var/www/services/urlshortener/client && cp -Rf common/ASC.UrlShortener/* /var/www/services/urlshortener/client && \
    cd common/services/ASC.Studio.Notify && \
    dotnet -d publish --no-build --self-contained -r linux-x64 -o /var/www/services/studio.notify

COPY config/mysql/conf.d/mysql.cnf /etc/mysql/conf.d/mysql.cnf
COPY config/supervisor/supervisord.conf /etc/supervisor/conf.d/supervisord.conf

COPY config/createdb.sql /app/onlyoffice/createdb.sql
COPY config/onlyoffice.sql /app/onlyoffice/onlyoffice.sql
COPY config/onlyoffice.data.sql /app/onlyoffice/onlyoffice.data.sql
COPY config/onlyoffice.resources.sql /app/onlyoffice/onlyoffice.resources.sql

RUN sed -i 's/Server=.*;Port=/Server=127.0.0.1;Port=/' /app/onlyoffice/config/appsettings.test.json

RUN mkdir -p /var/mysqld/ && \
    chown -R mysql:mysql /var/lib/mysql /var/run/mysqld /var/mysqld/ && \
    sudo -u mysql bash -c "/usr/bin/pidproxy /var/mysqld/mysqld.pid /usr/bin/mysqld_safe --pid-file=/var/mysqld/mysqld.pid &" && \
    sleep 5s && \
    mysql -e "CREATE DATABASE IF NOT EXISTS onlyoffice CHARACTER SET utf8 COLLATE 'utf8_general_ci'" && \
    mysql -D "onlyoffice" < /app/onlyoffice/createdb.sql && \
    mysql -D "onlyoffice" < /app/onlyoffice/onlyoffice.sql && \
    mysql -D "onlyoffice" < /app/onlyoffice/onlyoffice.data.sql && \
    mysql -D "onlyoffice" < /app/onlyoffice/onlyoffice.resources.sql && \
    mysql -D "onlyoffice" -e 'CREATE USER IF NOT EXISTS "onlyoffice_user"@"localhost" IDENTIFIED WITH mysql_native_password BY "onlyoffice_pass";' && \
    mysql -D "onlyoffice" -e 'GRANT ALL PRIVILEGES ON *.* TO 'onlyoffice_user'@'localhost';' && \
    killall -u mysql -n mysql

RUN rm -rf /var/lib/apt/lists/*

VOLUME /var/lib/mysql

EXPOSE 80 443 8092 8081

ENTRYPOINT ["/usr/bin/supervisord", "--"]
