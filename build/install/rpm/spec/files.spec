%files

%files api
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/studio/api/
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll
/etc/systemd/system/appserver-api.service
%dir /var/www/studio
%dir /var/www/products/
%dir /var/www/products/ASC.People/
%dir /var/www/products/ASC.People/server/
%dir /var/www/products/ASC.Files/
%dir /var/www/products/ASC.Files/server/

%files backup
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/services/backup/
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll
/etc/systemd/system/appserver-backup.service
%dir /var/www/services/
%dir /var/www/products/
%dir /var/www/products/ASC.People/
%dir /var/www/products/ASC.People/server/
%dir /var/www/products/ASC.Files/
%dir /var/www/products/ASC.Files/server/

%files common
%defattr(-, onlyoffice, onlyoffice, -)
%config /etc/onlyoffice/appserver/
/var/log/onlyoffice/appserver/
%config %attr(644, root, root) /usr/bin/*
%dir /etc/onlyoffice/
%dir /var/log/onlyoffice/

%files files_services
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/products/ASC.Files/service/
/etc/systemd/system/appserver-files_service.service
%dir /var/www/products/
%dir /var/www/products/ASC.Files/

%files notify
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/services/notify/
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll
/etc/systemd/system/appserver-notify.service
%dir /var/www/services/
%dir /var/www/products/
%dir /var/www/products/ASC.People/
%dir /var/www/products/ASC.People/server/
%dir /var/www/products/ASC.Files/
%dir /var/www/products/ASC.Files/server/

%files files
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/products/ASC.Files/server/
/var/www/products/ASC.People/server/ASC.People.dll
/etc/systemd/system/appserver-files.service
%dir /var/www/products/
%dir /var/www/products/ASC.People/
%dir /var/www/products/ASC.People/server/
%dir /var/www/products/ASC.Files/

%files api_system
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/services/apisystem/
/etc/systemd/system/appserver-api_system.service
%dir /var/www/services/

%files proxy
%defattr(-, onlyoffice, onlyoffice, -)
/etc/nginx/includes/*
/etc/nginx/conf.d/*
/var/www/story/
/var/www/products/ASC.People/client/
/var/www/products/ASC.Files/client/
/var/www/public/
/var/www/studio/client/
%dir /var/www/studio/
%dir /var/www/products/ASC.People/
%dir /var/www/products/ASC.Files/

%files studio.notify
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/services/studio.notify/
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll
/etc/systemd/system/appserver-studio_notify.service
%dir /var/www/products/
%dir /var/www/products/ASC.People/
%dir /var/www/products/ASC.People/server/
%dir /var/www/products/ASC.Files/
%dir /var/www/services/

%files people.server
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/products/ASC.People/server/
/var/www/products/ASC.Files/server/ASC.Files*.dll
/etc/systemd/system/appserver-people.service
%dir /var/www/products/
%dir /var/www/products/ASC.People/
%dir /var/www/products/ASC.Files/
%dir /var/www/products/ASC.Files/server/

%files urlshortener
%defattr(-, onlyoffice, onlyoffice, -)
/services/urlshortener/service/
/services/urlshortener/client/
/etc/systemd/system/appserver-urlshortener.service
%dir /services/
%dir /services/urlshortener/

%files thumbnails
%defattr(-, onlyoffice, onlyoffice, -)
/services/thumb/service/
/services/thumb/client/
/etc/systemd/system/appserver-thumbnails.service
%dir /services/
%dir /services/thumb/

%files socket
%defattr(-, onlyoffice, onlyoffice, -)
/services/socket/service/
/services/ASC.Socket.IO/
/etc/systemd/system/appserver-socket.service
%dir /services/
%dir /services/socket/

%files studio
%defattr(-, onlyoffice, onlyoffice, -)
/var/www/studio/server/
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll
/etc/systemd/system/appserver-studio.service
%dir /var/www/studio/
%dir /var/www/products/
%dir /var/www/products/ASC.People/
%dir /var/www/products/ASC.People/server/
%dir /var/www/products/ASC.Files/
%dir /var/www/products/ASC.Files/server/
