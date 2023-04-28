%package        backup
Summary:        Backup
Group:          Applications/Internet
Requires:       %name-common  = %version-%release 
Requires:       dotnet-sdk-7.0
AutoReqProv:    no
%description    backup
Backup

%package        common
Summary:        Common
Group:          Applications/Internet
Requires:       logrotate
%description    common
Common

%package        files-services
Summary:        Files-services
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-7.0
Requires:       ffmpeg
AutoReqProv:    no
%description    files-services
Files-services

%package        notify
Summary:        Notify
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-7.0
AutoReqProv:    no
%description    notify
Notify

%package        files
Summary:        Files
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-7.0
AutoReqProv:    no
%description    files
Files

%package        proxy
Summary:        Proxy
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nginx >= 1.9.5
Requires:       mysql-community-client >= 5.7.0
AutoReqProv:    no
%description    proxy
Proxy

%package        studio-notify
Summary:        Studio-notify
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-7.0
AutoReqProv:    no
%description    studio-notify
Studio-notify

%package        people-server
Summary:        People-server
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-7.0
AutoReqProv:    no
%description    people-server
People-server

%package        socket
Summary:        Socket
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nodejs >= 16.0
AutoReqProv:    no
%description    socket
Socket

%package        studio
Summary:        Studio
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-7.0
AutoReqProv:    no
%description    studio
Studio

%package        api
Summary:        Api
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-7.0
AutoReqProv:    no
%description    api
Api

%package        api-system
Summary:        Api-system
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-7.0
AutoReqProv:    no
%description    api-system
Api-system

%package        ssoauth
Summary:        Ssoauth
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nodejs >= 16.0
AutoReqProv:    no
%description    ssoauth
Ssoauth

%package        clear-events
Summary:        Clear-events
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-7.0
AutoReqProv:    no
%description    clear-events
Clear-events

%package        backup-background
Summary:        Backup-background
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-7.0
AutoReqProv:    no
%description    backup-background
Backup-background

%package        radicale
Summary:        Radicale
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       python3 >= 3.6
AutoReqProv:    no
%description    radicale
Radicale

%package        doceditor
Summary:        Doceditor
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nodejs >= 16.0
AutoReqProv:    no
%description    doceditor
Doceditor

%package        migration-runner
Summary:        Migration-runner
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-7.0
AutoReqProv:    no
%description    migration-runner
Migration-runner

%package        login
Summary:        Login
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nodejs >= 16.0
AutoReqProv:    no
%description    login
Login

%package        healthchecks
Summary:        Healthchecks
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-7.0
AutoReqProv:    no
%description    healthchecks
Healthchecks
