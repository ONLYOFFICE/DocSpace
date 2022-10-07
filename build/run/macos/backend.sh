#!/bin/bash

dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Root directory:" $dir

$dir/api.sh & 
$dir/studio.sh & 
$dir/people.sh & 
$dir/files.sh & 
$dir/files.service.sh &