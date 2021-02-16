%install
mkdir -p "%{buildroot}%{_bindir}/"
mkdir -p "%{buildroot}%{_sysconfdir}/nginx/conf.d/"
mkdir -p "%{buildroot}%{_sysconfdir}/nginx/includes"
mkdir -p "%{buildroot}%{_sysconfdir}/onlyoffice/appserver/config/"
mkdir -p "%{buildroot}%{_sysconfdir}/onlyoffice/appserver/data/"
mkdir -p "%{buildroot}%{_sysconfdir}/onlyoffice/appserver/.private/"
mkdir -p "%{buildroot}%{_sysconfdir}/systemd/system/"
mkdir -p "%{buildroot}%{_var}/log/onlyoffice/appserver/"
mkdir -p "%{buildroot}%{_var}/www/products/ASC.Files/client/"
mkdir -p "%{buildroot}%{_var}/www/products/ASC.Files/server/"
mkdir -p "%{buildroot}%{_var}/www/products/ASC.Files/service/"
mkdir -p "%{buildroot}%{_var}/www/products/ASC.People/client/"
mkdir -p "%{buildroot}%{_var}/www/products/ASC.People/server/"
mkdir -p "%{buildroot}%{_var}/www/public/"
mkdir -p "%{buildroot}%{_var}/www/services/apisystem/"
mkdir -p "%{buildroot}%{_var}/www/services/backup/"
mkdir -p "%{buildroot}%{_var}/www/services/notify/"
mkdir -p "%{buildroot}%{_var}/www/services/studio.notify/"
mkdir -p "%{buildroot}%{_var}/www/story/"
mkdir -p "%{buildroot}%{_var}/www/studio/api/"
mkdir -p "%{buildroot}%{_var}/www/studio/client/"
mkdir -p "%{buildroot}%{_var}/www/studio/server/"
mkdir -p "%{buildroot}/services/ASC.Socket.IO"
mkdir -p "%{buildroot}/services/socket/service"
mkdir -p "%{buildroot}/services/thumb/client/"
mkdir -p "%{buildroot}/services/thumb/service/"
mkdir -p "%{buildroot}/services/urlshortener/client/"
mkdir -p "%{buildroot}/services/urlshortener/service/"
mkdir -p "%{buildroot}%{_var}/www/products/ASC.Files/client/products/files"
mkdir -p "%{buildroot}%{_var}/www/products/ASC.People/client/products/people"
cp -rf %{_builddir}%{_var}/www/products/ASC.Files/server/* "%{buildroot}%{_var}/www/products/ASC.Files/server/"
cp -rf %{_builddir}%{_var}/www/products/ASC.Files/service/* "%{buildroot}%{_var}/www/products/ASC.Files/service/"
cp -rf %{_builddir}%{_var}/www/products/ASC.People/server/* "%{buildroot}%{_var}/www/products/ASC.People/server/"
cp -rf %{_builddir}%{_var}/www/services/apisystem/* "%{buildroot}%{_var}/www/services/apisystem/"
cp -rf %{_builddir}%{_var}/www/services/backup/* "%{buildroot}%{_var}/www/services/backup/"
cp -rf %{_builddir}%{_var}/www/services/notify/* "%{buildroot}%{_var}/www/services/notify/"
cp -rf %{_builddir}%{_var}/www/services/studio.notify/* "%{buildroot}%{_var}/www/services/studio.notify/"
cp -rf %{_builddir}%{_var}/www/studio/api/* "%{buildroot}%{_var}/www/studio/api"
cp -rf %{_builddir}%{_var}/www/studio/server/* "%{buildroot}%{_var}/www/studio/server/"
cp -rf %{_builddir}/AppServer-%GIT_BRANCH/build/install/docker/config/*.sql %{buildroot}%{_sysconfdir}/onlyoffice/appserver/
cp -rf %{_builddir}/AppServer-%GIT_BRANCH/build/install/rpm/*.sh %{buildroot}%{_bindir}/
cp -rf %{_builddir}/AppServer-%GIT_BRANCH/build/install/common/systemd/modules/* %{buildroot}%{_sysconfdir}/systemd/system
cp -rf %{_builddir}/AppServer-%GIT_BRANCH/common/ASC.Socket.IO/* %{buildroot}/services/ASC.Socket.IO
cp -rf %{_builddir}/AppServer-%GIT_BRANCH/common/ASC.Thumbnails/* %{buildroot}/services/thumb/client
cp -rf %{_builddir}/AppServer-%GIT_BRANCH/common/ASC.UrlShortener/* %{buildroot}/services/urlshortener/client
cp -rf %{_builddir}/AppServer-%GIT_BRANCH/config/* %{buildroot}%{_sysconfdir}/onlyoffice/appserver/config/
cp -rf %{_builddir}/AppServer-%GIT_BRANCH/config/nginx/includes/onlyoffice*.conf %{buildroot}%{_sysconfdir}/nginx/includes/
cp -rf %{_builddir}/AppServer-%GIT_BRANCH/config/nginx/onlyoffice*.conf %{buildroot}%{_sysconfdir}/nginx/conf.d/
cp -rf %{_builddir}/AppServer-%GIT_BRANCH/products/ASC.Files/Client/build/* %{buildroot}%{_var}/www/products/ASC.Files/client
cp -rf %{_builddir}/AppServer-%GIT_BRANCH/products/ASC.People/Client/build/* %{buildroot}%{_var}/www/products/ASC.People/client
cp -rf %{_builddir}/AppServer-%GIT_BRANCH/public/* %{buildroot}%{_var}/www/public/ 
cp -rf %{_builddir}/AppServer-%GIT_BRANCH/web/ASC.Web.Client/build/* %{buildroot}%{_var}/www/studio/client
cp -rf %{_builddir}/AppServer-%GIT_BRANCH/web/ASC.Web.Components/storybook-static/* %{buildroot}%{_var}/www/story/
cp -rf %{_builddir}/services/socket/service/* "%{buildroot}/services/socket/service/"
cp -rf %{_builddir}/services/thumb/service/* "%{buildroot}/services/thumb/service/"
cp -rf %{_builddir}/services/urlshortener/service/* "%{buildroot}/services/urlshortener/service/"
