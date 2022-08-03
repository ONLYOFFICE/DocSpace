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
    
  ForEach ( $item in $prereq_list ) {
    $url = $item.link
    $output = $path + $item.name

    if( $item.download_allways ){
      [system.console]::WriteLine("Downloading $url")
      Invoke-WebRequest -Uri $url -OutFile $output
    } else {
      if(![System.IO.File]::Exists($output)){
        [system.console]::WriteLine("Downloading $url")
        Invoke-WebRequest -Uri $url -OutFile $output
      }
    }
  }
}

$nginx_version = '1.21.1'

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
)

DownloadComponents $prerequisites $path_prereq
