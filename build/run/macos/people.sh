#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $rd

dir=$(builtin cd $rd/../../../; pwd)
echo "Root directory:" $dir

# Web API People
# set servicepath=%cd%\products\ASC.People\Server\bin\Debug\ASC.People.exe urls=http://0.0.0.0:5004 $STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=people pathToConf=%cd%\config core:products:folder=%cd%\products
dotnet $dir/products/ASC.People/Server/bin/Debug/ASC.People.dll urls=http://0.0.0.0:5004 $STORAGE_ROOT=$dir/Data log:dir=$dir/Logs log:name=people pathToConf=$dir/config core:products:folder=$dir/products