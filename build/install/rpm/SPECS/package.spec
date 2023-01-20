%package        backup
Summary:        Backup
Group:          Applications/Internet
Requires:       %name-common  = %version-%release 
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    backup

%package        common
Summary:        Common
Group:          Applications/Internet
%description    common

%package        files-services
Summary:        Files-services
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    files-services

%package        notify
Summary:        Notify
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    notify

%package        files
Summary:        Files
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    files

%package        proxy
Summary:        Proxy
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nginx >= 1.9.5
Requires:       mysql-community-client >= 5.7.0
AutoReqProv:    no
%description    proxy

%package        studio-notify
Summary:        Studio-notify
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    studio-notify

%package        people-server
Summary:        People-server
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    people-server

%package        socket
Summary:        Socket
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nodejs >= 16.0
AutoReqProv:    no
%description    socket

%package        studio
Summary:        Studio
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    studio

%package        api
Summary:        Api
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    api

%package        api-system
Summary:        Api-system
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    api-system

%package        ssoauth
Summary:        Ssoauth
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nodejs >= 16.0
AutoReqProv:    no
%description    ssoauth

%package        clear-events
Summary:        Clear-events
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    clear-events

%package        backup-background
Summary:        Backup-background
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    backup-background

%package        radicale
Summary:        Radicale
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       python3 >= 3.6
AutoReqProv:    no
%description    radicale

%package        doceditor
Summary:        Doceditor
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nodejs >= 16.0
AutoReqProv:    no
%description    doceditor

%package        migration-runner
Summary:        Migration-runner
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    migration-runner

%package        login
Summary:        Login
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nodejs >= 16.0
AutoReqProv:    no
%description    login
