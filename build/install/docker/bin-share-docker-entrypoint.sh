#!/bin/sh

echo "##################################################################"
echo "#####    Run preparation for launching AppServer services    #####"
echo "##################################################################"
cp -r /app/appserver/ASC.Files/server/* /var/www/products/ASC.Files/server/
cp -r /app/appserver/ASC.People/server/* /var/www/products/ASC.People/server/
cp -r /app/appserver/ASC.CRM/server/* /var/www/products/ASC.CRM/server/
cp -r /app/appserver/ASC.Projects/server/* /var/www/products/ASC.Projects/server/
cp -r /app/appserver/ASC.Calendar/server/* /var/www/products/ASC.Calendar/server/
cp -r /app/appserver/ASC.Mail/server/* /var/www/products/ASC.Mail/server/
chown -R onlyoffice:onlyoffice /var/www/products/
echo "Ok" > /var/www/products/ASC.Files/server/status.txt
echo "Preparation for launching AppServer services is complete"
