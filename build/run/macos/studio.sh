#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $rd

dir=$(builtin cd $rd/../../../; pwd)
echo "Root directory:" $dir

# Web Studio
# set servicepath=%cd%\web\ASC.Web.Studio\bin\Debug\ASC.Web.Studio.exe urls=http://0.0.0.0:5003 $STORAGE_ROOT=%cd%\Data  log:dir=%cd%\Logs log:name=studio pathToConf=%cd%\config core:products:folder=%cd%\products
dotnet $dir/web/ASC.Web.Studio/bin/Debug/ASC.Web.Studio.dll urls=http://0.0.0.0:5003 $STORAGE_ROOT=$dir/Data log:dir=$dir/Logs log:name=studio pathToConf=$dir/config core:products:folder=$dir/products