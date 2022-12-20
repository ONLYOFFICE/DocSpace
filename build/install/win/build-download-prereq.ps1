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

$path_prereq = "${pwd}\build\install\win\"

$prerequisites = @(
  @{  
    download_allways = $false; 
    name = "nginx-1.21.1.zip"; 
    link = "https://nginx.org/download/nginx-1.21.1.zip";
  }

  @{  
    download_allways = $false; 
    name = "apache-zookeeper-3.7.0-bin.tar.gz"; 
    link = "https://archive.apache.org/dist/zookeeper/zookeeper-3.7.0/apache-zookeeper-3.7.0-bin.tar.gz";
  }
  @{  
    download_allways = $false; 
    name = "kafka_2.12-2.8.0.tgz"; 
    link = "https://archive.apache.org/dist/kafka/2.8.0/kafka_2.12-2.8.0.tgz";
  }

  @{  
    download_allways = $false; 
    name = "WinSW.NET4new.exe"; 
    link = "https://github.com/winsw/winsw/releases/download/v2.11.0/WinSW.NET4.exe";
  }
)

DownloadComponents $prerequisites $path_prereq
