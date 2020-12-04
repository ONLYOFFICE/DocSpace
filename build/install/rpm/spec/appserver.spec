%define _unpackaged_files_terminate_build 0
%undefine _missing_build_ids_terminate_build
%global GIT_BRANCH develop
Name:           onlyoffice-appserver
Summary:        AppServer
Version:        0.0.0
Release:        0
Group:          Applications/Internet
URL:            http://onlyoffice.com
Vendor:         Ascensio System SIA
Packager:       Ascensio System SIA <support@onlyoffice.com>
ExclusiveArch:  x86_64
AutoReq:        no
AutoProv:       no
License:        GPLv3
Source0:        https://github.com/ONLYOFFICE/appserver/archive/%GIT_BRANCH.tar.gz
BuildRequires:  nodejs >= 10.0
BuildRequires:  yarn
BuildRequires:  libgdiplus
BuildRequires:  dotnet-sdk-3.1
Requires:       onlyoffice-appserver-backup
Requires:       onlyoffice-appserver-files_services
Requires:       onlyoffice-appserver-notify
Requires:       onlyoffice-appserver-files
Requires:       onlyoffice-appserver-api_system
Requires:       onlyoffice-appserver-proxy
Requires:       onlyoffice-appserver-people.server
Requires:       onlyoffice-appserver-urlshortener
Requires:       onlyoffice-appserver-thumbnails
Requires:       onlyoffice-appserver-studio
Requires:       onlyoffice-appserver-api
%description

%include package.spec

%prep

%setup -n AppServer-%GIT_BRANCH

%include build.spec

%include install.spec

%clean

%include files.spec

%pre

%pre common

addgroup --system --gid 107 onlyoffice && \
adduser -uid 104 --quiet --home /var/www/onlyoffice --system --gid 107 onlyoffice

%post

%post proxy

chown nginx:nginx /etc/nginx/* -R
sudo sed -e 's/#//' -i /etc/nginx/conf.d/onlyoffice.conf

mkdir -p /var/mysqld/ && \
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

%preun

%postun

%changelog
