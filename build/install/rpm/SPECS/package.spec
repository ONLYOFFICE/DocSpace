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

%package        socket
Summary:        socket
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nodejs >= 16.0
AutoReqProv:    no
%description    socket

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

%package        ssoauth
Summary:        ssoauth
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nodejs >= 16.0
AutoReqProv:    no
%description    ssoauth

%package        clear-events
Summary:        clear-events
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    clear-events

%package        backup-background
Summary:        backup-background
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    backup-background

%package        radicale
Summary:        radicale
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       python3 >= 3.6
AutoReqProv:    no
%description    radicale

%package        doceditor
Summary:        doceditor
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nodejs >= 16.0
AutoReqProv:    no
%description    doceditor

%package        migration-runner
Summary:        migration-runner
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       dotnet-sdk-6.0
AutoReqProv:    no
%description    migration-runner

%package        login
Summary:        login
Group:          Applications/Internet
Requires:       %name-common  = %version-%release
Requires:       nodejs >= 16.0
AutoReqProv:    no
%description    login
