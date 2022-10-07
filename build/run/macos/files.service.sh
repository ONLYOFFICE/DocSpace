#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $rd

dir=$(builtin cd $rd/../../../; pwd)
echo "Root directory:" $dir

# Web API Files.Service
# set servicepath=%cd%\products\ASC.Files\Service\bin\Debug\ASC.Files.Service.exe urls=http://0.0.0.0:5009 $STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=files.service pathToConf=%cd%\config core:products:folder=%cd%\products
dotnet $dir/products/ASC.Files/Service/bin/Debug/ASC.Files.Service.dll urls=http://0.0.0.0:5009 $STORAGE_ROOT=$dir/Data log:dir=$dir/Logs log:name=files.service pathToConf=$dir/config core:products:folder=$dir/products