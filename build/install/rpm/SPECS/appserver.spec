%global         product appserver
%global         buildpath %{_var}/www/%{product}
%global         sourcename AppServer-%GIT_BRANCH

Name:           onlyoffice-appserver
Summary:        Business productivity tools.
Group:          Applications/Internet
Version:        %version
Release:        %release
ExclusiveArch:  x86_64
AutoReqProv:    no

URL:            http://onlyoffice.com
Vendor:         Ascensio System SIA
Packager:       Ascensio System SIA <support@onlyoffice.com>
License:        AGPLv3

Source0:        https://github.com/ONLYOFFICE/%{product}/archive/%GIT_BRANCH.tar.gz#/%{sourcename}.tar.gz
Source1:        https://github.com/ONLYOFFICE/document-templates/archive/main/community-server.tar.gz#/document-templates-main-community-server.tar.gz
Source2:        https://github.com/ONLYOFFICE/dictionaries/archive/master.tar.gz#/dictionaries-master.tar.gz
Source3:        https://github.com/ONLYOFFICE/CommunityServer/archive/master.tar.gz#/CommunityServer-master.tar.gz

BuildRequires:  nodejs >= 12.0
BuildRequires:  yarn
BuildRequires:  dotnet-sdk-5.0

Requires:       %name-api-system
Requires:       %name-calendar
Requires:       %name-crm
Requires:       %name-backup
Requires:       %name-storage-encryption
Requires:       %name-storage-migration
Requires:       %name-files
Requires:       %name-files-services
Requires:       %name-mail
Requires:       %name-notify
Requires:       %name-people-server
Requires:       %name-projects-server
Requires:       %name-socket
Requires:       %name-ssoauth
Requires:       %name-studio-notify
Requires:       %name-telegram-service
Requires:       %name-thumbnails
Requires:       %name-urlshortener
Requires:       %name-api
Requires:       %name-studio
Requires:       %name-proxy

%description
App Server is a platform for building your own online office by connecting ONLYOFFICE modules packed as separate apps.

%include package.spec

%prep

rm -rf %{_rpmdir}/%{_arch}/%{name}-*
%setup -b1 -b2 -b3 -n %{sourcename}
mv -f %{_builddir}/document-templates-main-community-server/*  %{_builddir}/%{sourcename}/products/ASC.Files/Server/DocStore/
mv -f %{_builddir}/dictionaries-master/*  %{_builddir}/%{sourcename}/common/Tests/Frontend.Translations.Tests/dictionaries/
mv -f %{_builddir}/CommunityServer-master/build/sql/*  %{_builddir}/%{sourcename}/build/install/docker/config/

%include build.spec

%include install.spec

%include files.spec

%pre

%pre common

getent group onlyoffice >/dev/null || groupadd -r onlyoffice
getent passwd onlyoffice >/dev/null || useradd -r -g onlyoffice -s /sbin/nologin onlyoffice

%post 

chmod +x %{_bindir}/%{product}-configuration.sh

%preun

%postun

%clean

rm -rf %{_builddir} %{buildroot} 

%changelog
