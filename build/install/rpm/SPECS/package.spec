%package        backup
Summary:        backup
Group:          Applications/Internet
Requires:       %name-common
Requires:       dotnet-sdk-3.1
AutoReqProv:    no
%description    backup

%package        common
Summary:        common
Group:          Applications/Internet
%description    common

%package        files_services
Summary:        files_services
Group:          Applications/Internet
Requires:       %name-common
Requires:       dotnet-sdk-3.1
AutoReqProv:    no
%description    files_services

%package        notify
Summary:        notify
Group:          Applications/Internet
Requires:       %name-common
Requires:       dotnet-sdk-3.1
AutoReqProv:    no
%description    notify

%package        files
Summary:        files
Group:          Applications/Internet
Requires:       %name-common
Requires:       dotnet-sdk-3.1
AutoReqProv:    no
%description    files

%package        api_system
Summary:        api_system
Group:          Applications/Internet
Requires:       %name-common
Requires:       dotnet-sdk-3.1
AutoReqProv:    no
%description    api_system

%package        proxy
Summary:        proxy
Group:          Applications/Internet
Requires:       %name-common
Requires:       nginx >= 1.9.5
Requires:       mysql-community-client >= 5.7.0
AutoReqProv:    no
%description    proxy

%package        studio.notify
Summary:        studio.notify
Group:          Applications/Internet
Requires:       %name-common
Requires:       dotnet-sdk-3.1
AutoReqProv:    no
%description    studio.notify

%package        people.server
Summary:        people.server
Group:          Applications/Internet
Requires:       %name-common
Requires:       dotnet-sdk-3.1
AutoReqProv:    no
%description    people.server

%package        urlshortener
Summary:        urlshortener
Group:          Applications/Internet
Requires:       %name-common
Requires:       dotnet-sdk-3.1
Requires:       nodejs >= 12.0
AutoReqProv:    no
%description    urlshortener

%package        socket
Summary:        socket
Group:          Applications/Internet
Requires:       %name-common
Requires:       dotnet-sdk-3.1
Requires:       nodejs >= 12.0
AutoReqProv:    no
%description    socket

%package        thumbnails
Summary:        thumbnails
Group:          Applications/Internet
Requires:       %name-common
Requires:       dotnet-sdk-3.1
Requires:       nodejs >= 12.0
AutoReqProv:    no
%description    thumbnails

%package        studio
Summary:        studio
Group:          Applications/Internet
Requires:       %name-common
Requires:       dotnet-sdk-3.1
AutoReqProv:    no
%description    studio

%package        api
Summary:        api
Group:          Applications/Internet
Requires:       %name-common
Requires:       dotnet-sdk-3.1
AutoReqProv:    no
%description    api
