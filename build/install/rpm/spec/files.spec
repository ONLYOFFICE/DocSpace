%files

%files api
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/studio/api/*
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll
/etc/systemd/system/appserver-api.service

%files backup
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/services/backup/*
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll
/etc/systemd/system/appserver-backup.service

%files common
%defattr(-, onlyoffice, onlyoffice, -)
/etc/onlyoffice/appserver/*
/var/log/onlyoffice/appserver/
/usr/bin/*

%files files_services
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/products/ASC.Files/service/*
/etc/systemd/system/appserver-files_service.service

%files notify
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/services/notify/*
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll
/etc/systemd/system/appserver-notify.service

%files files
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/products/ASC.Files/server/*
/var/www/products/ASC.People/server/ASC.People.dll
/etc/systemd/system/appserver-files.service

%files api_system
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/services/apisystem/*
/etc/systemd/system/appserver-api_system.service

%files proxy
%defattr(-, onlyoffice, onlyoffice, -)
/etc/nginx/includes/*
/etc/nginx/conf.d/*
/etc/nginx/templates/upstream.conf.template
/var/www/story/*
/var/www/products/ASC.People/client/*
/var/www/products/ASC.Files/client/*
/var/www/public/*
/var/www/studio/client/*

%files studio.notify
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/services/studio.notify/*
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll
/etc/systemd/system/appserver-studio_notify.service

%files people.server
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/products/ASC.People/server/*
/var/www/products/ASC.Files/server/ASC.Files*.dll
/etc/systemd/system/appserver-people.service

%files urlshortener
%defattr(-, onlyoffice, onlyoffice, -)
/services/urlshortener/service/*
/services/urlshortener/client/*
/etc/systemd/system/appserver-urlshortener.service

%files thumbnails
%defattr(-, onlyoffice, onlyoffice, -)
/services/thumb/service/*
/services/thumb/client/*
/etc/systemd/system/appserver-thumbnails.service

%files socket
%defattr(-, onlyoffice, onlyoffice, -)
/services/socket/service/*
/services/ASC.Socket.IO/*
/etc/systemd/system/appserver-socket.service

%files studio
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/studio/server/*
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll
/etc/systemd/system/appserver-studio.service
