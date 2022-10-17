#!/bin/bash

dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Root directory:" $dir

$dir/stop.backend.docker.sh

$dir/start.backend.docker.sh