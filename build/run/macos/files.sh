#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $rd

dir=$(builtin cd $rd/../../../; pwd)
echo "Root directory:" $dir

# Web API Files
# set servicepath=%cd%\products\ASC.Files\Server\bin\Debug\ASC.Files.exe urls=http://0.0.0.0:5007 $STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=files pathToConf=%cd%\config core:products:folder=%cd%\products
dotnet $dir/products/ASC.Files/Server/bin/Debug/ASC.Files.dll urls=http://0.0.0.0:5007 $STORAGE_ROOT=$dir/Data log:dir=$dir/Logs log:name=files pathToConf=$dir/config core:products:folder=$dir/products