$AllProtocols = [System.Net.SecurityProtocolType]'Ssl3,Tls,Tls11,Tls12,Tls13'
[System.Net.ServicePointManager]::SecurityProtocol = $AllProtocols

# Function 'DownloadComponents' downloads some components that need on build satge
#
# It gets two parameters list of maps and download path
#
# The map consists of: download_allways ($true/$false) - should this component should download every time
#                  name - name of the dowmloaded component
#                  link - component download link

function DownloadComponents {

  param ( $prereq_list, $path )

  [void](New-Item -ItemType Directory -Force -Path $path)
    
  ForEach ( $item in $prereq_list ) {
    $url = $item.link
    $output = $path + $item.name

    try
    {
      if( $item.download_allways ){
        [system.console]::WriteLine("Downloading $url")
        Invoke-WebRequest -Uri $url -OutFile $output
      } else {
        if(![System.IO.File]::Exists($output)){
          [system.console]::WriteLine("Downloading $url")
          Invoke-WebRequest -Uri $url -OutFile $output
        }
      }
    } catch {
      Write-Host "[ERROR] Can not download" $item.name "by link" $url
    }
  }
}

switch ( $env:DOCUMENT_SERVER_VERSION_EE )
{
  latest { $DOCUMENT_SERVER_EE_LINK = "https://download.onlyoffice.com/install/documentserver/windows/onlyoffice-documentserver-ee.exe" }
  custom { $DOCUMENT_SERVER_EE_LINK = $env:DOCUMENT_SERVER_EE_CUSTOM_LINK.Replace(",", "") }
}

switch ( $env:DOCUMENT_SERVER_VERSION_CE )
{
  latest { $DOCUMENT_SERVER_CE_LINK = "https://download.onlyoffice.com/install/documentserver/windows/onlyoffice-documentserver.exe" }
  custom { $DOCUMENT_SERVER_CE_LINK = $env:DOCUMENT_SERVER_CE_CUSTOM_LINK.Replace(",", "") }
}

$nginx_version = '1.21.1'
$psql_version = '14.0'

$path_prereq = "${pwd}\build\install\win\"

$prerequisites = @(
  @{  
    download_allways = $false; 
    name = "nginx-${nginx_version}.zip";
    link = "https://nginx.org/download/nginx-${nginx_version}.zip";
  }

  @{  
    download_allways = $false; 
    name = "WinSW.NET4new.exe"; 
    link = "https://github.com/winsw/winsw/releases/download/v2.11.0/WinSW.NET4.exe";
  }

   @{  
    # Downloading onlyoffice-documentserver-ee for DocSpace Enterprise
    download_allways = $true; 
    name = "onlyoffice-documentserver-ee.exe"; 
    link = $DOCUMENT_SERVER_EE_LINK
  }

  @{
    # Downloading onlyoffice-documentserver for DocSpace Community
    download_allways = $true; 
    name = "onlyoffice-documentserver.exe"; 
    link = $DOCUMENT_SERVER_CE_LINK
  }
   
  @{  
    download_allways = $false; 
    name = "psqlodbc_15_x64.msi";
    link = "http://download.onlyoffice.com/install/windows/redist/psqlodbc_15_x64.msi"
  }

  @{  
    download_allways = $false; 
    name = "postgresql-${psql_version}-1-windows-x64.exe"; 
    link = "https://get.enterprisedb.com/postgresql/postgresql-${psql_version}-1-windows-x64.exe"
  }
)

$path_nuget_packages = "${pwd}\.nuget\packages\"

$nuget_packages = @(
  @{  
    download_allways = $false; 
    name = "rabbitmq.client.3.6.5.nupkg"; 
    link = "https://www.nuget.org/api/v2/package/RabbitMQ.Client/3.6.5";
  }
)

$path_enterprise_prereq = "${pwd}\build\install\win\redist\"

$enterprise_prerequisites = @(
  @{
    download_allways = $false;
    name = ".net_framework_4.8.exe";
    link = "https://download.visualstudio.microsoft.com/download/pr/014120d7-d689-4305-befd-3cb711108212/0fd66638cde16859462a6243a4629a50/ndp48-x86-x64-allos-enu.exe"
  }

  @{
    download_allways = $false;
    name = "aspnetcore-runtime-7.0.4-win-x64.exe";
    link = "https://download.visualstudio.microsoft.com/download/pr/1c260404-69d2-4c07-979c-644846ba1f46/7d27639ac67f1e502b83a738406da0ee/aspnetcore-runtime-7.0.4-win-x64.exe";
  }

  @{
    download_allways = $false;
    name = "dotnet-runtime-7.0.4-win-x64.exe";
    link = "https://download.visualstudio.microsoft.com/download/pr/7e842a78-9877-4b82-8450-f3311b406a6f/83352282a0bdf1e5f9dfc5fcc88dc83f/dotnet-runtime-7.0.4-win-x64.exe";
  }

  @{
    download_allways = $false;
    name = "vcredist_2013u5_x64.exe";
    link = "http://download.microsoft.com/download/C/C/2/CC2DF5F8-4454-44B4-802D-5EA68D086676/vcredist_x64.exe";
  }

   @{
    download_allways = $false;
    name = "VC_redist.x86.exe";
    link = "https://download.visualstudio.microsoft.com/download/pr/d60aa805-26e9-47df-b4e3-cd6fcc392333/A06AAC66734A618AB33C1522920654DDFC44FC13CAFAA0F0AB85B199C3D51DC0/VC_redist.x86.exe";
  }

  @{
    download_allways = $false;
    name = "VC_redist.x64.exe";
    link = "https://download.visualstudio.microsoft.com/download/pr/d60aa805-26e9-47df-b4e3-cd6fcc392333/7D7105C52FCD6766BEEE1AE162AA81E278686122C1E44890712326634D0B055E/VC_redist.x64.exe";
  }

  @{  
    download_allways = $false; 
    name = "mysql-connector-odbc-8.0.33-win32.msi";
    link = "https://cdn.mysql.com/Downloads/Connector-ODBC/8.0/mysql-connector-odbc-8.0.33-win32.msi";
  }

  @{  
    download_allways = $false; 
    name = "mysql-installer-community-8.0.33.0.msi";
    link = "https://cdn.mysql.com/Downloads/MySQLInstaller/mysql-installer-community-8.0.33.0.msi";
  }

  @{  
    download_allways = $false; 
    name = "node-v18.16.1-x64.msi"; 
    link = "https://nodejs.org/dist/v18.16.1/node-v18.16.1-x64.msi";
  }

  @{  
    download_allways = $false; 
    name = "elasticsearch-7.10.0.msi";
    link = "https://artifacts.elastic.co/downloads/elasticsearch/elasticsearch-7.10.0.msi";
  }

  @{  
    download_allways = $false; 
    name = "otp_win64_26.0.2.exe";
    link = "https://github.com/erlang/otp/releases/download/OTP-26.0.2/otp_win64_26.0.2.exe";
  }

  @{  
    download_allways = $false; 
    name = "rabbitmq-server-3.12.1.exe";
    link = "https://github.com/rabbitmq/rabbitmq-server/releases/download/v3.12.1/rabbitmq-server-3.12.1.exe";
  }

  @{  
    download_allways = $false; 
    name = "Redis-x64-5.0.10.msi"; 
    link = "http://download.onlyoffice.com/install/windows/redist/Redis-x64-5.0.10.msi";
  }

  @{  
    download_allways = $false; 
    name = "FFmpeg_Essentials.msi"; 
    link = "https://github.com/icedterminal/ffmpeg-installer/releases/download/6.0.0.20230306/FFmpeg_Essentials.msi";
  }

  @{  
    download_allways = $false; 
    name = "psqlodbc_15_x64.msi";
    link = "http://download.onlyoffice.com/install/windows/redist/psqlodbc_15_x64.msi"
  }

  @{  
    download_allways = $false; 
    name = "postgresql-${psql_version}-1-windows-x64.exe"; 
    link = "https://get.enterprisedb.com/postgresql/postgresql-${psql_version}-1-windows-x64.exe"
  }
)

DownloadComponents $prerequisites $path_prereq

DownloadComponents $nuget_packages $path_nuget_packages

DownloadComponents $enterprise_prerequisites $path_enterprise_prereq