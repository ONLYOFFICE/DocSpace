%define         _binaries_in_noarch_packages_terminate_build   0
%define         _build_id_links none

%global         product docspace
%global         buildpath %{_var}/www/%{product}
%global         sourcename DocSpace-%GIT_BRANCH

Name:           %{product}
Summary:        Business productivity tools
Group:          Applications/Internet
Version:        %{version}
Release:        %{release}

AutoReqProv:    no

BuildArch:      noarch
URL:            http://onlyoffice.com
Vendor:         Ascensio System SIA
Packager:       %{packager}
License:        AGPLv3

Source0:        https://github.com/ONLYOFFICE/%{product}/archive/%GIT_BRANCH.tar.gz#/%{sourcename}.tar.gz
Source1:        https://github.com/ONLYOFFICE/document-templates/archive/main/community-server.tar.gz#/document-templates-main-community-server.tar.gz
Source2:        https://github.com/ONLYOFFICE/dictionaries/archive/master.tar.gz#/dictionaries-master.tar.gz
Source3:        %{product}.rpmlintrc

BuildRequires:  nodejs >= 18.0
BuildRequires:  yarn
BuildRequires:  dotnet-sdk-7.0

BuildRoot:      %_tmppath/%name-%version-%release.%arch

Requires:       %name-api = %version-%release
Requires:       %name-api-system = %version-%release
Requires:       %name-backup = %version-%release
Requires:       %name-backup-background = %version-%release
Requires:       %name-clear-events = %version-%release
Requires:       %name-doceditor = %version-%release
Requires:       %name-files = %version-%release
Requires:       %name-files-services = %version-%release
Requires:       %name-healthchecks = %version-%release
Requires:       %name-login = %version-%release
Requires:       %name-migration-runner = %version-%release
Requires:       %name-notify = %version-%release
Requires:       %name-people-server = %version-%release
Requires:       %name-proxy = %version-%release
Requires:       %name-radicale = %version-%release
Requires:       %name-socket = %version-%release
Requires:       %name-ssoauth = %version-%release
Requires:       %name-studio = %version-%release
Requires:       %name-studio-notify = %version-%release

%description
ONLYOFFICE DocSpace is a new way to collaborate on documents with teams, 
clients, partners, etc., based on the concept of rooms - special spaces with 
predefined permissions. 

%include package.spec

%prep

rm -rf %{_rpmdir}/%{_arch}/%{name}-*
%setup -b1 -b2 -n %{sourcename} -q
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

%preun

%postun

%clean

rm -rf %{_builddir} %{buildroot} 

%changelog
*Mon Jan 16 2023 %{packager} - %{version}-%{release}
- Initial build.

%triggerin radicale -- python3, python36

if ! which python3; then
   if rpm -q python36; then
      update-alternatives --install /usr/bin/python3 python3 /usr/bin/python3.6 1
   fi
fi

python3 -m pip install --upgrade radicale
python3 -m pip install --upgrade %{buildpath}/Tools/radicale/plugins/app_auth_plugin/.
python3 -m pip install --upgrade %{buildpath}/Tools/radicale/plugins/app_store_plugin/.
python3 -m pip install --upgrade %{buildpath}/Tools/radicale/plugins/app_rights_plugin/.
