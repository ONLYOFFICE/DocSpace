$AllProtocols = [System.Net.SecurityProtocolType]'Ssl3,Tls,Tls11,Tls12,Tls13'
[System.Net.ServicePointManager]::SecurityProtocol = $AllProtocols

# Function 'DownloadComponents' downloads some components that need on build satge
#
# It gets two parameters list of maps and download path
#
# The map consists of: download_allways ($true/$false) - should this component should download every time
#                  name - name of the dowmloaded component
#                  link - component download link

# Функция 'DownloadComponents' скачивает некоторые компоненты, которые нужны при сборке
#
# Принимает два параметра: список хэш-таблиц и путь загрузки
#
# Хэш-таблица состоит из: download_allways ($true/$false) - должен ли этот компонент скачиваться каждый раз
#                         name - имя загружаемого компонента
#                         link - ссылка на скачивание компонента

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
    link = "https://dlcdn.apache.org/zookeeper/zookeeper-3.7.0/apache-zookeeper-3.7.0-bin.tar.gz";
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

$path_sql = "${pwd}\build\install\docker\config\"

$sql_community = @(
  @{  
    download_allways = $true; 
    name = "onlyoffice.sql"; 
    link = "https://raw.githubusercontent.com/ONLYOFFICE/CommunityServer/master/build/sql/onlyoffice.sql";
  }

  @{  
    download_allways = $true; 
    name = "onlyoffice.data.sql"; 
    link = "https://raw.githubusercontent.com/ONLYOFFICE/CommunityServer/master/build/sql/onlyoffice.data.sql";
  }
  @{  
    download_allways = $true; 
    name = "onlyoffice.upgradev110.sql"; 
    link = "https://raw.githubusercontent.com/ONLYOFFICE/CommunityServer/master/build/sql/onlyoffice.upgradev110.sql";
  }

  @{  
    download_allways = $true; 
    name = "onlyoffice.upgradev111.sql"; 
    link = "https://raw.githubusercontent.com/ONLYOFFICE/CommunityServer/master/build/sql/onlyoffice.upgradev111.sql";
  }

  @{  
    download_allways = $true; 
    name = "onlyoffice.upgradev115.sql"; 
    link = "https://raw.githubusercontent.com/ONLYOFFICE/CommunityServer/master/build/sql/onlyoffice.upgradev115.sql";
  }

  @{  
    download_allways = $true; 
    name = "onlyoffice.upgradev116.sql"; 
    link = "https://raw.githubusercontent.com/ONLYOFFICE/CommunityServer/master/build/sql/onlyoffice.upgradev116.sql";
  }
)

DownloadComponents $prerequisites $path_prereq

DownloadComponents $sql_community $path_sql