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
Requires:       %name-ASC.ApiSystem
Requires:       %name-ASC.CRM
Requires:       %name-ASC.Data.Backup
Requires:       %name-ASC.Data.Storage.Encryption
Requires:       %name-ASC.Files
Requires:       %name-ASC.Files.Service
Requires:       %name-ASC.Notify
Requires:       %name-ASC.People
Requires:       %name-ASC.Projects
Requires:       %name-ASC.Socket
Requires:       %name-ASC.Studio.Notify
Requires:       %name-ASC.TelegramService
Requires:       %name-ASC.Thumbnails.Svc
Requires:       %name-ASC.UrlShortener.Svc
Requires:       %name-ASC.Web.Api
Requires:       %name-ASC.Web.Studio
Requires:       %name-Proxy
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

%pre Common

getent group onlyoffice >/dev/null || groupadd -r onlyoffice
getent passwd onlyoffice >/dev/null || useradd -r -g onlyoffice -s /sbin/nologin onlyoffice

%post 

chmod +x %{_bindir}/appserver-configuration.sh

%preun

%postun

%clean

rm -rf %{buildroot}

%changelog
