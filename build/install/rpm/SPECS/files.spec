%files
%config %attr(644, root, root) %{_bindir}/*

%files api
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/studio/api/
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_sysconfdir}/systemd/system/appserver-api.service
%dir %{_var}/www/appserver/studio
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/

%files backup
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/services/backup/
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_sysconfdir}/systemd/system/appserver-backup.service
%dir %{_var}/www/appserver/services/
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/

%files common
%defattr(-, onlyoffice, onlyoffice, -)
%config %{_sysconfdir}/onlyoffice/appserver/
%{_var}/log/onlyoffice/appserver/
%{_var}/www/appserver/sql/
%dir %{_sysconfdir}/onlyoffice/
%dir %{_var}/log/onlyoffice/

%files files_services
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/products/ASC.Files/service/
%{_sysconfdir}/systemd/system/appserver-files_service.service
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.Files/

%files notify
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/services/notify/
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_sysconfdir}/systemd/system/appserver-notify.service
%dir %{_var}/www/appserver/services/
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/

%files files
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/products/ASC.Files/server/
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_sysconfdir}/systemd/system/appserver-files.service
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server/
%dir %{_var}/www/appserver/products/ASC.Files/

%files api_system
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/services/apisystem/
%{_sysconfdir}/systemd/system/appserver-api_system.service
%dir %{_var}/www/appserver/services/

%files proxy
%defattr(-, onlyoffice, onlyoffice, -)
%{_sysconfdir}/nginx/includes/*
%{_sysconfdir}/nginx/conf.d/*
%{_var}/www/appserver/story/
%{_var}/www/appserver/products/ASC.People/client/
%{_var}/www/appserver/products/ASC.Files/client/
%{_var}/www/appserver/public/
%{_var}/www/appserver/studio/client/
%dir %{_var}/www/appserver/studio/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.Files/

%files studio.notify
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/services/studio.notify/
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_sysconfdir}/systemd/system/appserver-studio_notify.service
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/services/

%files people.server
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/products/ASC.People/server/
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_sysconfdir}/systemd/system/appserver-people.service
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/

%files urlshortener
%defattr(-, onlyoffice, onlyoffice, -)
/services/ASC.UrlShortener/
%{_sysconfdir}/systemd/system/appserver-urlshortener.service
%dir /services/
%dir /services/ASC.UrlShortener/
%dir /services/ASC.UrlShortener/service

%files thumbnails
%defattr(-, onlyoffice, onlyoffice, -)
/services/ASC.Thumbnails/
%{_sysconfdir}/systemd/system/appserver-thumbnails.service
%dir /services/
%dir /services/ASC.Thumbnails/
%dir /services/ASC.Thumbnails/service

%files socket
%defattr(-, onlyoffice, onlyoffice, -)
/services/ASC.Socket.IO/
%{_sysconfdir}/systemd/system/appserver-socket.service
%dir /services/
%dir /services/ASC.Socket.IO/
%dir /services/ASC.Socket.IO/service

%files studio
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/studio/server/
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_sysconfdir}/systemd/system/appserver-studio.service
%dir %{_var}/www/appserver/studio/
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/
