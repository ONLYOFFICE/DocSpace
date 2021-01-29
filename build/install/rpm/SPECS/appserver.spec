%define         debug_package %{nil}
Name:           onlyoffice-appserver
Summary:        AppServer
Version:        %version
Release:        %release
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
Requires:       %name-backup
Requires:       %name-files_services
Requires:       %name-notify
Requires:       %name-files
Requires:       %name-api_system
Requires:       %name-proxy
Requires:       %name-people.server
Requires:       %name-urlshortener
Requires:       %name-thumbnails
Requires:       %name-studio
Requires:       %name-api
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

chmod +x %{_bindir}/appserver-configuracion.sh

%preun

%postun

%changelog
