#!/bin/bash
CHANGES=$(/snap/libxml2/current/bin/xmllint --xpath '//changeSet/item/affectedPath/text()' $1);
shift
for i in $CHANGES
do
  for j in $@
  do
   if [[ $i == $j* ]]; then
    echo 1
    exit
   fi
  done
done

echo 0
exit

