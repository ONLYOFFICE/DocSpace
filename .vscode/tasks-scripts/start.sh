#!/bin/bash

scriptLocation=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
root=$(builtin cd $scriptLocation/../../; pwd)

if [ "$1" = "Start" ]
  then (sh $root/build/start/start.backend.docker.sh)
elif [ "$1" = "Restart" ]
  then (sh $root/build/start/restart.backend.docker.sh)
elif [ "$1" = "Stop" ]
  then (sh $root/build/start/stop.backend.docker.sh)
fi