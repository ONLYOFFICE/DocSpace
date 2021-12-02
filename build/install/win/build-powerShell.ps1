######## Downloading components ########

$AllProtocols = [System.Net.SecurityProtocolType]'Ssl3,Tls,Tls11,Tls12,Tls13'
[System.Net.ServicePointManager]::SecurityProtocol = $AllProtocols

$hash_table = @{"nginx-1.21.1.zip"="https://nginx.org/download/nginx-1.21.1.zip";
                "apache-zookeeper-3.7.0-bin.tar.gz"="https://dlcdn.apache.org/zookeeper/zookeeper-3.7.0/apache-zookeeper-3.7.0-bin.tar.gz";
                "kafka_2.12-2.8.0.tgz"="https://archive.apache.org/dist/kafka/2.8.0/kafka_2.12-2.8.0.tgz";
                "WinSW.NET4new.exe"="https://github.com/winsw/winsw/releases/download/v2.11.0/WinSW.NET4.exe"} 

ForEach ($key in $hash_table.keys) {
    $url = $hash_table[$key]
    $output = "${pwd}\build\install\win\" + $key

    if(![System.IO.File]::Exists($output)){
        [system.console]::WriteLine("Downloading $url")
        Invoke-WebRequest -Uri $url -OutFile $output
    }
}