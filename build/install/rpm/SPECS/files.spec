%files

%files api
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/studio/api/
%{_var}/www/products/ASC.People/server/ASC.People.dll
%{_var}/www/products/ASC.Files/server/ASC.Files*.dll
%{_sysconfdir}/systemd/system/appserver-api.service
%dir %{_var}/www/studio
%dir %{_var}/www/products/
%dir %{_var}/www/products/ASC.People/
%dir %{_var}/www/products/ASC.People/server/
%dir %{_var}/www/products/ASC.Files/
%dir %{_var}/www/products/ASC.Files/server/

%files backup
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/services/backup/
%{_var}/www/products/ASC.People/server/ASC.People.dll
%{_var}/www/products/ASC.Files/server/ASC.Files*.dll
%{_sysconfdir}/systemd/system/appserver-backup.service
%dir %{_var}/www/services/
%dir %{_var}/www/products/
%dir %{_var}/www/products/ASC.People/
%dir %{_var}/www/products/ASC.People/server/
%dir %{_var}/www/products/ASC.Files/
%dir %{_var}/www/products/ASC.Files/server/

%files common
%defattr(-, onlyoffice, onlyoffice, -)
%config %{_sysconfdir}/onlyoffice/appserver/
%{_var}/log/onlyoffice/appserver/
%config %attr(644, root, root) %{_bindir}/*
%dir %{_sysconfdir}/onlyoffice/
%dir %{_var}/log/onlyoffice/

%files files_services
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/products/ASC.Files/service/
%{_sysconfdir}/systemd/system/appserver-files_service.service
%dir %{_var}/www/products/
%dir %{_var}/www/products/ASC.Files/

%files notify
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/services/notify/
%{_var}/www/products/ASC.People/server/ASC.People.dll
%{_var}/www/products/ASC.Files/server/ASC.Files*.dll
%{_sysconfdir}/systemd/system/appserver-notify.service
%dir %{_var}/www/services/
%dir %{_var}/www/products/
%dir %{_var}/www/products/ASC.People/
%dir %{_var}/www/products/ASC.People/server/
%dir %{_var}/www/products/ASC.Files/
%dir %{_var}/www/products/ASC.Files/server/

%files files
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/products/ASC.Files/server/
%{_var}/www/products/ASC.People/server/ASC.People.dll
%{_sysconfdir}/systemd/system/appserver-files.service
%dir %{_var}/www/products/
%dir %{_var}/www/products/ASC.People/
%dir %{_var}/www/products/ASC.People/server/
%dir %{_var}/www/products/ASC.Files/

%files api_system
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/services/apisystem/
%{_sysconfdir}/systemd/system/appserver-api_system.service
%dir %{_var}/www/services/

%files proxy
%defattr(-, onlyoffice, onlyoffice, -)
%{_sysconfdir}/nginx/includes/*
%{_sysconfdir}/nginx/conf.d/*
%{_var}/www/story/
%{_var}/www/products/ASC.People/client/
%{_var}/www/products/ASC.Files/client/
%{_var}/www/public/
%{_var}/www/studio/client/
%dir %{_var}/www/studio/
%dir %{_var}/www/products/ASC.People/
%dir %{_var}/www/products/ASC.Files/

%files studio.notify
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/services/studio.notify/
%{_var}/www/products/ASC.People/server/ASC.People.dll
%{_var}/www/products/ASC.Files/server/ASC.Files*.dll
%{_sysconfdir}/systemd/system/appserver-studio_notify.service
%dir %{_var}/www/products/
%dir %{_var}/www/products/ASC.People/
%dir %{_var}/www/products/ASC.People/server/
%dir %{_var}/www/products/ASC.Files/
%dir %{_var}/www/services/

%files people.server
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/products/ASC.People/server/
%{_var}/www/products/ASC.Files/server/ASC.Files*.dll
%{_sysconfdir}/systemd/system/appserver-people.service
%dir %{_var}/www/products/
%dir %{_var}/www/products/ASC.People/
%dir %{_var}/www/products/ASC.Files/
%dir %{_var}/www/products/ASC.Files/server/

%files urlshortener
%defattr(-, onlyoffice, onlyoffice, -)
/services/urlshortener/service/
/services/urlshortener/client/
%{_sysconfdir}/systemd/system/appserver-urlshortener.service
%dir /services/
%dir /services/urlshortener/

%files thumbnails
%defattr(-, onlyoffice, onlyoffice, -)
/services/thumb/service/
/services/thumb/client/
%{_sysconfdir}/systemd/system/appserver-thumbnails.service
%dir /services/
%dir /services/thumb/

%files socket
%defattr(-, onlyoffice, onlyoffice, -)
/services/socket/service/
/services/ASC.Socket.IO/
%{_sysconfdir}/systemd/system/appserver-socket.service
%dir /services/
%dir /services/socket/

%files studio
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/studio/server/
%{_var}/www/products/ASC.People/server/ASC.People.dll
%{_var}/www/products/ASC.Files/server/ASC.Files*.dll
%{_sysconfdir}/systemd/system/appserver-studio.service
%dir %{_var}/www/studio/
%dir %{_var}/www/products/
%dir %{_var}/www/products/ASC.People/
%dir %{_var}/www/products/ASC.People/server/
%dir %{_var}/www/products/ASC.Files/
%dir %{_var}/www/products/ASC.Files/server/
