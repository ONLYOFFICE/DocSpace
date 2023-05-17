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
$psql_version = '12.9'

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
    name = "psqlodbc_x64.msi"; 
    link = "http://download.onlyoffice.com/install/windows/redist/psqlodbc_x64.msi"
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

DownloadComponents $prerequisites $path_prereq

DownloadComponents $nuget_packages $path_nuget_packages
