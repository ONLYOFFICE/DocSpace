%define _unpackaged_files_terminate_build 0
%global GIT_BRANCH develop
Name:           onlyoffice-appserver
Summary:        AppServer
Version:        0.0.0
Release:        0
Group:          Applications/Internet
URL:            http://onlyoffice.com
Vendor:         Ascensio System SIA
Packager:       Ascensio System SIA <support@onlyoffice.com>
BuildArch:      x86_64
AutoReq:        no
AutoProv:       no
License:        GPLv3
Source0:        https://github.com/ONLYOFFICE/appserver/archive/%GIT_BRANCH.tar.gz
BuildRequires:  nodejs
BuildRequires:  yarn
BuildRequires:  libgdiplus
BuildRequires:  dotnet-sdk-3.1

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

%preun

%postun

%changelog
