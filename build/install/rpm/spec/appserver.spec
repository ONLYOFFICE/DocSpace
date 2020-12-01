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
Source0:        https://github.com/ONLYOFFICE/appserver/archive/develop.tar.gz
BuildRequires:  nodejs
BuildRequires:  yarn
BuildRequires:  libgdiplus
BuildRequires:  dotnet-sdk-3.1
%description

%include %package

%prep

rm -rf %{_builddir}/app/onlyoffice/src
mkdir -p %{_builddir}/app/onlyoffice/ && cd %{_builddir}/app/onlyoffice/
gzip -dc %{_sourcedir}/%GIT_BRANCH.tar.gz | tar -xvvf -
if [ $? -ne 0 ]; then
  exit $?
fi
mv AppServer-%GIT_BRANCH src

%build

%include %build

%install

%clean

%files

%include %files

%pre

# add defualt user and group for no-root run
groupadd -g 1001 appuser && \
useradd -r -u 1001 -g appuser appuser && \
chown appuser:appuser %{buildroot}/app/onlyoffice -R && \
chown appuser:appuser %{buildroot}%{_localstatedir}/log -R  && \
chown appuser:appuser %{buildroot}%{_localstatedir}/www -R 

%post

%preun

%postun

%changelog
