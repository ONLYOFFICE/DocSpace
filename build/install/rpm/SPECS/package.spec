%package        backup
Summary:        backup
Group:          Applications/Internet
Requires:       %name-common  = %version-%release 
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    backup

%package        common
Summary:        common
Group:          Applications/Internet
%description    common

%package        files-services
Summary:        files-services
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    files-services

%package        notify
Summary:        notify
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    notify

%package        files
Summary:        files
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    files

%package        api-system
Summary:        api-system
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    api-system

%package        proxy
Summary:        proxy
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nginx >= 1.9.5
Requires:       mysql-community-client >= 5.7.0
AutoReqProv:    no
%description    proxy

%package        studio-notify
Summary:        studio-notify
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    studio-notify

%package        people-server
Summary:        people-server
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    people-server

%package        urlshortener
Summary:        urlshortener
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
Requires:       nodejs >= 12.0
AutoReqProv:    no
%description    urlshortener

%package        socket
Summary:        socket
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
Requires:       nodejs >= 12.0
AutoReqProv:    no
%description    socket

%package        thumbnails
Summary:        thumbnails
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
Requires:       nodejs >= 12.0
AutoReqProv:    no
%description    thumbnails

%package        studio
Summary:        studio
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    studio

%package        api
Summary:        api
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    api

%package        storage-encryption
Summary:        storage-encryption
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    storage-encryption

%package        storage-migration
Summary:        storage-migration
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    storage-migration

%package        telegram-service
Summary:        telegram-service
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    telegram-service

%package        ssoauth
Summary:        ssoauth
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
Requires:       nodejs >= 12.0
AutoReqProv:    no
%description    ssoauth
