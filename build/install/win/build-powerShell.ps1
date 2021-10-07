######## Downloading components ########

$AllProtocols = [System.Net.SecurityProtocolType]'Ssl3,Tls,Tls11,Tls12,Tls13'
[System.Net.ServicePointManager]::SecurityProtocol = $AllProtocols

$names_array = @("nginx","zookeeper","kafka","winsw")

$URL_hash_table = @{"nginx"="https://nginx.org/download/nginx-1.21.1.zip";
                    "zookeeper"="https://dlcdn.apache.org/zookeeper/zookeeper-3.7.0/apache-zookeeper-3.7.0-bin.tar.gz";
                    "kafka"="https://archive.apache.org/dist/kafka/2.8.0/kafka_2.12-2.8.0.tgz";
                    "winsw"="https://github.com/winsw/winsw/releases/download/v2.11.0/WinSW.NET4.exe"} 

$Names_hash_table = @{"nginx"="nginx-1.21.1.zip";
                        "zookeeper"="apache-zookeeper-3.7.0-bin.tar.gz";
                        "kafka"="kafka_2.12-2.8.0.tgz";
                        "winsw"="WinSW.NET4new.exe"} 

ForEach ($name in $names_array) {
    $url = $URL_hash_table.$name
    $output = "${pwd}\build\install\win\" + $Names_hash_table.$name

    if(![System.IO.File]::Exists($output)){
        [system.console]::WriteLine("Downloading $url")
        Invoke-WebRequest -Uri $url -OutFile $output
    }
}