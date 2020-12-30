%define _unpackaged_files_terminate_build 0
%undefine _missing_build_ids_terminate_build
%define  debug_package %{nil}
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
AutoReqProv:    no
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

getent group onlyoffice >/dev/null || groupadd -r onlyoffice
getent passwd onlyoffice >/dev/null || useradd -r -g onlyoffice -s /sbin/nologin onlyoffice

%post

%post common

chmod +x /usr/bin/appserver-configuracion.sh

%preun

%postun

%changelog
