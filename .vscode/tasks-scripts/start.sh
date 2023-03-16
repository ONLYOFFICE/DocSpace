#!/bin/bash

scriptLocation=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
root=$(builtin cd $scriptLocation/../../; pwd)

if [ "$1" = "Start" ]
  then (cd $root/build/start && sh ./start.backend.docker.sh)
elif [ "$1" = "Restart" ]
  then (cd $root/build/start && sh ./restart.backend.docker.sh)
elif [ "$1" = "Stop" ]
  then (cd $root/build/start && sh ./stop.backend.docker.sh)
fi