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

BuildRequires:  nodejs >= 14.0
BuildRequires:  yarn
BuildRequires:  dotnet-sdk-6.0

Requires:       %name-api-system = %version-%release
Requires:       %name-backup = %version-%release
Requires:       %name-storage-encryption = %version-%release
Requires:       %name-storage-migration = %version-%release
Requires:       %name-files = %version-%release
Requires:       %name-files-services = %version-%release
Requires:       %name-notify = %version-%release
Requires:       %name-people-server = %version-%release
Requires:       %name-socket = %version-%release
Requires:       %name-ssoauth = %version-%release
Requires:       %name-studio-notify = %version-%release
Requires:       %name-telegram-service = %version-%release
Requires:       %name-thumbnails = %version-%release
Requires:       %name-urlshortener = %version-%release
Requires:       %name-api = %version-%release
Requires:       %name-studio = %version-%release
Requires:       %name-proxy = %version-%release

%description
App Server is a platform for building your own online office by connecting ONLYOFFICE modules packed as separate apps.

%include package.spec

%prep

rm -rf %{_rpmdir}/%{_arch}/%{name}-*
%setup -b1 -b2 -n %{sourcename}
mv -f %{_builddir}/document-templates-main-community-server/*  %{_builddir}/%{sourcename}/products/ASC.Files/Server/DocStore/
mv -f %{_builddir}/dictionaries-master/*  %{_builddir}/%{sourcename}/common/Tests/Frontend.Translations.Tests/dictionaries/

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
