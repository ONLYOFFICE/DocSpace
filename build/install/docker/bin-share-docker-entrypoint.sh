#!/bin/sh

echo "##################################################################"
echo "#####    Run preparation for launching DocSpace services     #####"
echo "##################################################################"
cp -r /app/ASC.Files/server/* /var/www/products/ASC.Files/server/
cp -r /app/ASC.People/server/* /var/www/products/ASC.People/server/
chown -R onlyoffice:onlyoffice /var/www/products/
echo "Ok" > /var/www/products/ASC.Files/server/status.txt
echo "Preparation for launching DocSpace services is complete"
