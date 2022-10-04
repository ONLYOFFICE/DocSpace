#!/bin/bash

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $rd

dir=$(builtin cd $rd/../../../; pwd)
echo "Root directory:" $dir

# Web API Root
dotnet $dir/web/ASC.Web.Api/bin/Debug/ASC.Web.Api.dll urls=http://0.0.0.0:5000 $STORAGE_ROOT=$dir/Data log:dir=$dir/Logs log:name=api pathToConf=$dir/config core:products:folder=$dir/products