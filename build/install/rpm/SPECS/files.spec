%files
%config %attr(644, root, root) %{_bindir}/*

%files ASC.Web.Api
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/studio/api/
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/appserver/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/appserver/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/AppServer-ASC.Web.Api.service
%dir %{_var}/www/appserver/studio/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.CRM/server/
%dir %{_var}/www/appserver/products/ASC.Projects/
%dir %{_var}/www/appserver/products/ASC.Projects/server/

%files ASC.Data.Backup
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/services/backup/
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/appserver/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/appserver/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/AppServer-ASC.Data.Backup.service
%dir %{_var}/www/appserver/services/
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.CRM/server/
%dir %{_var}/www/appserver/products/ASC.Projects/
%dir %{_var}/www/appserver/products/ASC.Projects/server/

%files Common
%defattr(-, onlyoffice, onlyoffice, -)
%config %{_sysconfdir}/onlyoffice/appserver/
%{_var}/log/onlyoffice/appserver/
%{_var}/www/appserver/sql/
%dir %{_sysconfdir}/onlyoffice/
%dir %{_var}/log/onlyoffice/

%files ASC.Files.Service
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/products/ASC.Files/service/
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/appserver/products/ASC.People/server/ASC.People*.dll
%{_var}/www/appserver/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/appserver/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/AppServer-ASC.Files.Service.service
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.CRM/server/
%dir %{_var}/www/appserver/products/ASC.Projects/
%dir %{_var}/www/appserver/products/ASC.Projects/server/

%files ASC.Notify
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/services/notify/
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/appserver/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/appserver/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/AppServer-ASC.Notify.service
%dir %{_var}/www/appserver/services/
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.CRM/server/
%dir %{_var}/www/appserver/products/ASC.Projects/
%dir %{_var}/www/appserver/products/ASC.Projects/server/

%files ASC.Files
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/products/ASC.Files/server/
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/appserver/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/AppServer-ASC.Files.service
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server/
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.CRM/server/
%dir %{_var}/www/appserver/products/ASC.Projects/
%dir %{_var}/www/appserver/products/ASC.Projects/server/

%files ASC.ApiSystem
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/services/apisystem/
%{_sysconfdir}/systemd/system/AppServer-ASC.ApiSystem.service
%dir %{_var}/www/appserver/services/

%files Proxy
%defattr(-, onlyoffice, onlyoffice, -)
%{_sysconfdir}/nginx/includes/*
%{_sysconfdir}/nginx/conf.d/*
%{_var}/www/appserver/public/
%{_var}/www/appserver/studio/client/
%{_var}/www/appserver/studio/login
%{_var}/www/appserver/products/ASC.People/client/
%{_var}/www/appserver/products/ASC.Files/client/
%{_var}/www/appserver/products/ASC.Files/editor
%{_var}/www/appserver/products/ASC.CRM/client/
%{_var}/www/appserver/products/ASC.Projects/client
%dir %{_var}/www/appserver/studio/
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.Projects/

%files ASC.Studio.Notify
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/services/studio.notify/
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/appserver/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/appserver/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/AppServer-ASC.Studio.Notify.service
%dir %{_var}/www/appserver/services/
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.CRM/server/
%dir %{_var}/www/appserver/products/ASC.Projects/
%dir %{_var}/www/appserver/products/ASC.Projects/server/

%files ASC.People
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/products/ASC.People/server/
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/appserver/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/appserver/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/AppServer-ASC.People.service
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.CRM/server/
%dir %{_var}/www/appserver/products/ASC.Projects/
%dir %{_var}/www/appserver/products/ASC.Projects/server/

%files ASC.UrlShortener.Svc
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/services/urlshortener/service/
%{_var}/www/appserver/services/urlshortener/client/
%{_sysconfdir}/systemd/system/AppServer-ASC.UrlShortener.Svc.service
%dir %{_var}/www/appserver/services/
%dir %{_var}/www/appserver/services/urlshortener/

%files ASC.Thumbnails.Svc
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/services/thumb/service/
%{_var}/www/appserver/services/thumb/client/
%{_sysconfdir}/systemd/system/AppServer-ASC.Thumbnails.Svc.service
%dir %{_var}/www/appserver/services/
%dir %{_var}/www/appserver/services/thumb/

%files ASC.Socket
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/services/ASC.Socket.IO
%{_var}/www/appserver/services/socket/service/
%{_var}/www/appserver/products/ASC.Files/server/
%{_var}/www/appserver/products/ASC.People/server/
%{_var}/www/appserver/products/ASC.CRM/server/
%{_var}/www/appserver/products/ASC.Projects/server/
%{_sysconfdir}/systemd/system/AppServer-ASC.Socket.service
%dir %{_var}/www/appserver/services/
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.Projects/

%files ASC.Web.Studio
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/studio/server/
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/appserver/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/appserver/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/AppServer-ASC.Web.Studio.service
%dir %{_var}/www/appserver/studio/
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.CRM/server/
%dir %{_var}/www/appserver/products/ASC.Projects/
%dir %{_var}/www/appserver/products/ASC.Projects/server/

%files ASC.Data.Storage.Encryption
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/services/storage.encryption/
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/appserver/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/AppServer-ASC.Data.Storage.Encryption.service
%dir %{_var}/www/appserver/services/
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.CRM/server
%dir %{_var}/www/appserver/products/ASC.Projects/
%dir %{_var}/www/appserver/products/ASC.Projects/server

%files ASC.Data.Storage.Migration
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/services/storage.migration/
%{_var}/www/appserver/products/ASC.Files/server/
%{_var}/www/appserver/products/ASC.People/server/
%{_var}/www/appserver/products/ASC.CRM/server/
%{_var}/www/appserver/products/ASC.Projects/server/
%{_sysconfdir}/systemd/system/AppServer-ASC.Data.Storage.Migration.service
%dir %{_var}/www/appserver/services/
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.Projects/

%files ASC.Projects
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/products/ASC.Projects/server/
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll 
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.CRM/server/ASC.CRM*.dll
%{_sysconfdir}/systemd/system/AppServer-ASC.Projects.service
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server/
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.CRM/server/
%dir %{_var}/www/appserver/products/ASC.Projects/

%files ASC.TelegramService
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/services/telegram/service/
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/appserver/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/AppServer-ASC.TelegramService.service
%dir %{_var}/www/appserver/services/
%dir %{_var}/www/appserver/services/telegram/
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/
%dir %{_var}/www/appserver/products/ASC.People/
%dir %{_var}/www/appserver/products/ASC.People/server/
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.CRM/server/
%dir %{_var}/www/appserver/products/ASC.Projects/
%dir %{_var}/www/appserver/products/ASC.Projects/server/

%files ASC.CRM
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/appserver/products/ASC.CRM/server/
%{_var}/www/appserver/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/appserver/products/ASC.People/server/ASC.People.dll
%{_var}/www/appserver/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/AppServer-ASC.CRM.service
%dir %{_var}/www/appserver/products/
%dir %{_var}/www/appserver/products/ASC.CRM/
%dir %{_var}/www/appserver/products/ASC.Files/
%dir %{_var}/www/appserver/products/ASC.Files/server/
%dir %{_var}/www/appserver/products/ASC.Projects/
%dir %{_var}/www/appserver/products/ASC.Projects/server/
