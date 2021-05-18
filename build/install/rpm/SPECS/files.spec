%files
%config %attr(644, root, root) %{_bindir}/*

%files api
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/studio/api/
%{_var}/www/%{sysname}/products/ASC.People/server/ASC.People.dll
%{_var}/www/%{sysname}/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/%{sysname}/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/%{sysname}/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/%{sysname}-api.service
%dir %{_var}/www/%{sysname}/studio/
%dir %{_var}/www/%{sysname}/products/ASC.People/
%dir %{_var}/www/%{sysname}/products/ASC.People/server/
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.Files/server/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/server/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/server/

%files backup
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/services/ASC.Data.Backup/
%{_var}/www/%{sysname}/products/ASC.People/server/ASC.People.dll
%{_var}/www/%{sysname}/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/%{sysname}/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/%{sysname}/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/%{sysname}-backup.service
%dir %{_var}/www/%{sysname}/services/
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.People/
%dir %{_var}/www/%{sysname}/products/ASC.People/server/
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.Files/server/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/server/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/server/

%files common
%defattr(-, onlyoffice, onlyoffice, -)
%config %{_sysconfdir}/onlyoffice/%{sysname}/
%{_var}/log/onlyoffice/%{sysname}/
%{_var}/www/%{sysname}/sql/
%dir %{_sysconfdir}/onlyoffice/
%dir %{_var}/log/onlyoffice/

%files files-services
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/products/ASC.Files/service/
%{_var}/www/%{sysname}/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/%{sysname}/products/ASC.People/server/ASC.People*.dll
%{_var}/www/%{sysname}/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/%{sysname}/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/%{sysname}-files-services.service
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.People/
%dir %{_var}/www/%{sysname}/products/ASC.People/server
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.Files/server/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/server/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/server/

%files notify
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/services/ASC.Notify/
%{_var}/www/%{sysname}/products/ASC.People/server/ASC.People.dll
%{_var}/www/%{sysname}/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/%{sysname}/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/%{sysname}/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/%{sysname}-notify.service
%dir %{_var}/www/%{sysname}/services/
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.People/
%dir %{_var}/www/%{sysname}/products/ASC.People/server/
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.Files/server/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/server/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/server/

%files files
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/products/ASC.Files/server/
%{_var}/www/%{sysname}/products/ASC.People/server/ASC.People.dll
%{_var}/www/%{sysname}/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/%{sysname}/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/%{sysname}-files.service
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.People/
%dir %{_var}/www/%{sysname}/products/ASC.People/server/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/server/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/server/

%files api-system
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/services/ASC.ApiSystem/
%{_sysconfdir}/systemd/system/%{sysname}-api-system.service
%dir %{_var}/www/%{sysname}/services/

%files proxy
%defattr(-, onlyoffice, onlyoffice, -)
%{_sysconfdir}/nginx/includes/*
%{_sysconfdir}/nginx/conf.d/*
%{_var}/www/%{sysname}/public/
%{_var}/www/%{sysname}/studio/client/
%{_var}/www/%{sysname}/studio/login
%{_var}/www/%{sysname}/products/ASC.People/client/
%{_var}/www/%{sysname}/products/ASC.Files/client/
%{_var}/www/%{sysname}/products/ASC.Files/editor/
%{_var}/www/%{sysname}/products/ASC.CRM/client/
%{_var}/www/%{sysname}/products/ASC.Projects/client
%{_var}/www/%{sysname}/products/ASC.Calendar/client/
%{_var}/www/%{sysname}/products/ASC.Mail/client
%dir %{_var}/www/%{sysname}/studio/
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.People/
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/
%dir %{_var}/www/%{sysname}/products/ASC.Calendar/
%dir %{_var}/www/%{sysname}/products/ASC.Mail/

%files studio-notify
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/services/ASC.Studio.Notify/
%{_var}/www/%{sysname}/products/ASC.People/server/ASC.People.dll
%{_var}/www/%{sysname}/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/%{sysname}/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/%{sysname}/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/%{sysname}-studio-notify.service
%dir %{_var}/www/%{sysname}/services/
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.People/
%dir %{_var}/www/%{sysname}/products/ASC.People/server/
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.Files/server/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/server/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/server/

%files people-server
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/products/ASC.People/server/
%{_var}/www/%{sysname}/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/%{sysname}/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/%{sysname}/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/%{sysname}-people-server.service
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.People/
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.Files/server/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/server/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/server/

%files urlshortener
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/services/ASC.UrlShortener/
%{_sysconfdir}/systemd/system/%{sysname}-urlshortener.service
%dir %{_var}/www/%{sysname}/services/

%files thumbnails
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/services/ASC.Thumbnails/
%{_sysconfdir}/systemd/system/%{sysname}-thumbnails.service
%dir %{_var}/www/%{sysname}/services/

%files socket
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/services/ASC.Socket.IO/
%{_var}/www/%{sysname}/products/ASC.Files/server/
%{_var}/www/%{sysname}/products/ASC.People/server/
%{_var}/www/%{sysname}/products/ASC.CRM/server/
%{_var}/www/%{sysname}/products/ASC.Projects/server/
%{_sysconfdir}/systemd/system/%{sysname}-socket.service
%dir %{_var}/www/%{sysname}/services/
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.People/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/

%files studio
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/studio/server/
%{_var}/www/%{sysname}/products/ASC.People/server/ASC.People.dll
%{_var}/www/%{sysname}/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/%{sysname}/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/%{sysname}/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/%{sysname}-studio.service
%dir %{_var}/www/%{sysname}/studio/
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.People/
%dir %{_var}/www/%{sysname}/products/ASC.People/server/
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.Files/server/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/server/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/server/

%files storage-encryption
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/services/ASC.Data.Storage.Encryption/
%{_var}/www/%{sysname}/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/%{sysname}/products/ASC.People/server/ASC.People.dll
%{_var}/www/%{sysname}/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/%{sysname}/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/%{sysname}-storage-encryption.service
%dir %{_var}/www/%{sysname}/services/
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.Files/server
%dir %{_var}/www/%{sysname}/products/ASC.People/
%dir %{_var}/www/%{sysname}/products/ASC.People/server
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/server
%dir %{_var}/www/%{sysname}/products/ASC.Projects/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/server

%files storage-migration
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/services/ASC.Data.Storage.Migration/
%{_var}/www/%{sysname}/products/ASC.Files/server/
%{_var}/www/%{sysname}/products/ASC.People/server/
%{_var}/www/%{sysname}/products/ASC.CRM/server/
%{_var}/www/%{sysname}/products/ASC.Projects/server/
%{_sysconfdir}/systemd/system/%{sysname}-storage-migration.service
%dir %{_var}/www/%{sysname}/services/
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.People/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/

%files projects-server
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/products/ASC.Projects/server/
%{_var}/www/%{sysname}/products/ASC.Files/server/ASC.Files*.dll 
%{_var}/www/%{sysname}/products/ASC.People/server/ASC.People.dll
%{_var}/www/%{sysname}/products/ASC.CRM/server/ASC.CRM*.dll
%{_sysconfdir}/systemd/system/%{sysname}-projects-server.service
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.Files/server/
%dir %{_var}/www/%{sysname}/products/ASC.People/
%dir %{_var}/www/%{sysname}/products/ASC.People/server/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/server/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/

%files telegram-service
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/services/ASC.TelegramService/
%{_var}/www/%{sysname}/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/%{sysname}/products/ASC.People/server/ASC.People.dll
%{_var}/www/%{sysname}/products/ASC.CRM/server/ASC.CRM*.dll
%{_var}/www/%{sysname}/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/%{sysname}-telegram-service.service
%dir %{_var}/www/%{sysname}/services/
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.Files/server/
%dir %{_var}/www/%{sysname}/products/ASC.People/
%dir %{_var}/www/%{sysname}/products/ASC.People/server/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/server/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/server/

%files crm
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/products/ASC.CRM/server/
%{_var}/www/%{sysname}/products/ASC.Files/server/ASC.Files*.dll
%{_var}/www/%{sysname}/products/ASC.People/server/ASC.People.dll
%{_var}/www/%{sysname}/products/ASC.Projects/server/ASC.Projects*.dll
%{_sysconfdir}/systemd/system/%{sysname}-crm.service
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.CRM/
%dir %{_var}/www/%{sysname}/products/ASC.Files/
%dir %{_var}/www/%{sysname}/products/ASC.Files/server/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/
%dir %{_var}/www/%{sysname}/products/ASC.Projects/server/

%files calendar
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/products/ASC.Calendar/server/
%{_sysconfdir}/systemd/system/%{sysname}-calendar.service
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.Calendar/

%files mail
%defattr(-, onlyoffice, onlyoffice, -)
%{_var}/www/%{sysname}/products/ASC.Mail/server/
%{_sysconfdir}/systemd/system/%{sysname}-mail.service
%dir %{_var}/www/%{sysname}/products/
%dir %{_var}/www/%{sysname}/products/ASC.Mail/
