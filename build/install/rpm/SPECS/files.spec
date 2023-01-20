%files
%attr(744, root, root) %{_bindir}/*

%files api
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/studio/ASC.Web.Api/
%{buildpath}/products/ASC.People/server/ASC.People.dll
%{buildpath}/products/ASC.Files/server/ASC.Files*.dll
/usr/lib/systemd/system/%{product}-api.service
%dir %{buildpath}/studio/
%dir %{buildpath}/products/ASC.People/
%dir %{buildpath}/products/ASC.People/server/
%dir %{buildpath}/products/ASC.Files/
%dir %{buildpath}/products/ASC.Files/server/

%files api-system
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/services/ASC.ApiSystem/
/usr/lib/systemd/system/%{product}-api-system.service
%dir %{buildpath}/services/

%files backup
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/services/ASC.Data.Backup/
%{buildpath}/products/ASC.People/server/ASC.People.dll
%{buildpath}/products/ASC.Files/server/ASC.Files*.dll
/usr/lib/systemd/system/%{product}-backup.service
%dir %{buildpath}/services/
%dir %{buildpath}/products/
%dir %{buildpath}/products/ASC.People/
%dir %{buildpath}/products/ASC.People/server/
%dir %{buildpath}/products/ASC.Files/
%dir %{buildpath}/products/ASC.Files/server/

%files common
%defattr(-, onlyoffice, onlyoffice, -)
%config %{_sysconfdir}/onlyoffice/%{product}/
%{_var}/log/onlyoffice/%{product}/
%dir %{_sysconfdir}/onlyoffice/
%dir %{_var}/log/onlyoffice/

%files files-services
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/products/ASC.Files/service/
%{buildpath}/products/ASC.Files/server/ASC.Files*.dll
%{buildpath}/products/ASC.People/server/ASC.People*.dll
/usr/lib/systemd/system/%{product}-files-services.service
%dir %{buildpath}/products/
%dir %{buildpath}/products/ASC.People/
%dir %{buildpath}/products/ASC.People/server
%dir %{buildpath}/products/ASC.Files/
%dir %{buildpath}/products/ASC.Files/server/

%files notify
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/services/ASC.Notify/
%{buildpath}/products/ASC.People/server/ASC.People.dll
%{buildpath}/products/ASC.Files/server/ASC.Files*.dll
/usr/lib/systemd/system/%{product}-notify.service
%dir %{buildpath}/services/
%dir %{buildpath}/products/
%dir %{buildpath}/products/ASC.People/
%dir %{buildpath}/products/ASC.People/server/
%dir %{buildpath}/products/ASC.Files/
%dir %{buildpath}/products/ASC.Files/server/

%files files
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/products/ASC.Files/server/
%{buildpath}/products/ASC.People/server/ASC.People.dll
/usr/lib/systemd/system/%{product}-files.service
%dir %{buildpath}/products/
%dir %{buildpath}/products/ASC.Files/
%dir %{buildpath}/products/ASC.People/
%dir %{buildpath}/products/ASC.People/server/

%files proxy
%defattr(-, onlyoffice, onlyoffice, -)
%{_sysconfdir}/nginx/includes/*
%{_sysconfdir}/nginx/conf.d/*
%{buildpath}/public/
%{buildpath}/client/

%files studio-notify
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/services/ASC.Studio.Notify/
%{buildpath}/products/ASC.People/server/ASC.People.dll
%{buildpath}/products/ASC.Files/server/ASC.Files*.dll
/usr/lib/systemd/system/%{product}-studio-notify.service
%dir %{buildpath}/services/
%dir %{buildpath}/products/
%dir %{buildpath}/products/ASC.People/
%dir %{buildpath}/products/ASC.People/server/
%dir %{buildpath}/products/ASC.Files/
%dir %{buildpath}/products/ASC.Files/server/

%files people-server
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/products/ASC.People/server/
%{buildpath}/products/ASC.Files/server/ASC.Files*.dll
/usr/lib/systemd/system/%{product}-people-server.service
%dir %{buildpath}/products/
%dir %{buildpath}/products/ASC.People/
%dir %{buildpath}/products/ASC.Files/
%dir %{buildpath}/products/ASC.Files/server/

%files socket
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/services/ASC.Socket.IO/
%{buildpath}/products/ASC.Files/server/ASC.Files*.dll
%{buildpath}/products/ASC.People/server/ASC.People.dll
/usr/lib/systemd/system/%{product}-socket.service
%dir %{buildpath}/services/
%dir %{buildpath}/products/
%dir %{buildpath}/products/ASC.Files/
%dir %{buildpath}/products/ASC.People/

%files studio
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/studio/ASC.Web.Studio/
%{buildpath}/products/ASC.People/server/ASC.People.dll
%{buildpath}/products/ASC.Files/server/ASC.Files*.dll
/usr/lib/systemd/system/%{product}-studio.service
%dir %{buildpath}/studio/
%dir %{buildpath}/products/
%dir %{buildpath}/products/ASC.People/
%dir %{buildpath}/products/ASC.People/server/
%dir %{buildpath}/products/ASC.Files/
%dir %{buildpath}/products/ASC.Files/server/

%files ssoauth
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/services/ASC.SsoAuth/
/usr/lib/systemd/system/%{product}-ssoauth.service
%dir %{buildpath}/services/

%files clear-events
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/services/ASC.ClearEvents/
/usr/lib/systemd/system/%{product}-clear-events.service
%dir %{buildpath}/services/

%files backup-background
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/services/ASC.Data.Backup.BackgroundTasks/
/usr/lib/systemd/system/%{product}-backup-background.service
%dir %{buildpath}/services/

%files radicale
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/Tools/radicale/
%dir %{buildpath}/Tools/

%files doceditor
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/products/ASC.Files/editor/
/usr/lib/systemd/system/%{product}-doceditor.service
%dir %{buildpath}/products/
%dir %{buildpath}/products/ASC.Files/

%files migration-runner
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/services/ASC.Migration.Runner/
/usr/lib/systemd/system/%{product}-migration-runner.service
%dir %{buildpath}/services/

%files login
%defattr(-, onlyoffice, onlyoffice, -)
%{buildpath}/products/ASC.Login/login
/usr/lib/systemd/system/%{product}-login.service
%dir %{buildpath}/products/
%dir %{buildpath}/products/ASC.Login/
