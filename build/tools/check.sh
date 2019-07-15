#!/bin/bash
CHANGES=$(/snap/libxml2/current/bin/xmllint --xpath '//changeSet/item/affectedPath/text()' $1);
shift
for i in $CHANGES
do
  for j in $@
  do
   if [[ $i == $j* ]]; then
    exit 0
   fi
  done
done

exit 1

