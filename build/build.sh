#!/bin/bash

echo off

echo "##########################################################"
echo "#########  Start build and deploy  #######################"
echo "##########################################################"

echo ""

rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $rd

dir=$(builtin cd $rd/../; pwd)
echo "Root directory:" $dir

pushd $dir

echo "FRONT-END (for start run command 'yarn start' inside the root folder)"
yarn install

echo "BACK-END"
dotnet build $dir/asc.web.sln  /fl1 /flp1:logfile=asc.web.log;verbosity=normal

echo "install nodejs projects dependencies..."
pushd $dir/common/ASC.Socket.IO/
yarn install
pushd $dir/common/ASC.SsoAuth/
yarn install
pushd $dir/common/ASC.WebDav/
yarn install
pushd $dir/common/ASC.UrlShortener/
yarn install
pushd $dir/common/ASC.WebPlugins/
yarn install