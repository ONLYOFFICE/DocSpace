%define         debug_package %{nil}
Name:           onlyoffice-appserver
Summary:        Business productivity tools.
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
BuildRequires:  nodejs >= 12.0
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
Requires:       %name-studio.notify
Requires:       %name-socket
Requires:       %name-api
AutoReqProv:    no
%description
App Server is a platform for building your own online office by connecting ONLYOFFICE modules packed as separate apps.

%include package.spec

%prep

rm -rf %{_rpmdir}/%{_arch}/%{name}-*
%setup -n AppServer-%GIT_BRANCH

%include build.spec

%include install.spec

%include files.spec

%pre

%pre common

getent group onlyoffice >/dev/null || groupadd -r onlyoffice
getent passwd onlyoffice >/dev/null || useradd -r -g onlyoffice -s /sbin/nologin onlyoffice

%post 

chmod +x %{_bindir}/appserver-configuration.sh

%preun

%postun

%clean

rm -rf %{buildroot}

%changelog
