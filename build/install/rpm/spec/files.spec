%files api
%defattr(-, root, root, -)
/var/www/studio/api/
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll

%files backup
%defattr(-, root, root, -)
/var/www/services/backup/
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll

%files config
%defattr(-, root, root, -)
/app/onlyoffice/config/

%files files_services
%defattr(-, root, root, -)
/var/www/products/ASC.Files/service/

%files notify
%defattr(-, root, root, -)
/var/www/services/notify/
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll

%files files
%defattr(-, root, root, -)
/var/www/products/ASC.Files/server/
/var/www/products/ASC.People/server/ASC.People.dll

%files api_system
%defattr(-, root, root, -)
/var/www/services/apisystem/

%files proxy
%defattr(-, root, root, -)
/etc/nginx/includes/
/var/www/story/
/var/www/products/ASC.People/client/
/var/www/products/ASC.Files/client/
/var/www/public/
/var/www/studio/client/
/etc/nginx/conf.d/
#/etc/nginx/templates/upstream.conf.template #???

%files studio.notify
%defattr(-, root, root, -)
/var/www/services/studio.notify/
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll

%files people.server
%defattr(-, root, root, -)
/var/www/products/ASC.People/server/
/var/www/products/ASC.Files/server/ASC.Files*.dll

%files urlshortener
%defattr(-, root, root, -)
/services/urlshortener/service/
/var/www/services/urlshortener/client/ #???

%files thumbnails
%defattr(-, root, root, -)
/services/thumb/service/
/var/www/services/thumb/client/ #???

%files studio
%defattr(-, root, root, -)
/var/www/studio/server/
/var/www/products/ASC.People/server/ASC.People.dll
/var/www/products/ASC.Files/server/ASC.Files*.dll
