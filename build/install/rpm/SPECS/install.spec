%install
mkdir -p "%{buildroot}%{_bindir}/"
mkdir -p "%{buildroot}%{_sysconfdir}/nginx/conf.d/"
mkdir -p "%{buildroot}%{_sysconfdir}/nginx/includes/"
mkdir -p "%{buildroot}%{_sysconfdir}/onlyoffice/%{product}/"
mkdir -p "%{buildroot}%{_sysconfdir}/onlyoffice/%{product}/.private/"
mkdir -p "%{buildroot}%{_sysconfdir}/onlyoffice/%{product}/data/"
mkdir -p "%{buildroot}%{_var}/log/onlyoffice/%{product}/"
mkdir -p "%{buildroot}/lib/systemd/system/"
mkdir -p "%{buildroot}%{buildpath}/products/ASC.Calendar/client/"
mkdir -p "%{buildroot}%{buildpath}/products/ASC.Calendar/server/"
mkdir -p "%{buildroot}%{buildpath}/products/ASC.CRM/client/"
mkdir -p "%{buildroot}%{buildpath}/products/ASC.CRM/server/"
mkdir -p "%{buildroot}%{buildpath}/products/ASC.Files/client/"
mkdir -p "%{buildroot}%{buildpath}/products/ASC.Files/editor/"
mkdir -p "%{buildroot}%{buildpath}/products/ASC.Files/server/DocStore/"
mkdir -p "%{buildroot}%{buildpath}/products/ASC.Files/service/"
mkdir -p "%{buildroot}%{buildpath}/products/ASC.Mail/client/"
mkdir -p "%{buildroot}%{buildpath}/products/ASC.Mail/server/"
mkdir -p "%{buildroot}%{buildpath}/products/ASC.People/client/"
mkdir -p "%{buildroot}%{buildpath}/products/ASC.People/server/"
mkdir -p "%{buildroot}%{buildpath}/products/ASC.Projects/client/"
mkdir -p "%{buildroot}%{buildpath}/products/ASC.Projects/server/"
mkdir -p "%{buildroot}%{buildpath}/public/"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.Socket.IO/"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.Socket.IO.Svc/"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.SsoAuth/"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.SsoAuth.Svc/"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.ApiSystem/"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.Data.Backup/"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.Notify/"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.Data.Storage.Encryption/service"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.Data.Storage.Migration/service"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.Studio.Notify/"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.TelegramService/"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.Thumbnails/"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.UrlShortener/"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.UrlShortener.Svc/"
mkdir -p "%{buildroot}%{buildpath}/services/ASC.Thumbnails.Svc/"
mkdir -p "%{buildroot}%{buildpath}/studio/api/"
mkdir -p "%{buildroot}%{buildpath}/studio/client/"
mkdir -p "%{buildroot}%{buildpath}/studio/login/"
mkdir -p "%{buildroot}%{buildpath}/studio/server/"
cp -rf %{_builddir}/%{sourcename}/publish/products/ASC.Calendar/server/* "%{buildroot}%{buildpath}/products/ASC.Calendar/server/"
cp -rf %{_builddir}/%{sourcename}/publish/products/ASC.Mail/server/* "%{buildroot}%{buildpath}/products/ASC.Mail/server/"
cp -rf %{_builddir}/%{sourcename}/publish/products/ASC.CRM/server/* "%{buildroot}%{buildpath}/products/ASC.CRM/server/"
cp -rf %{_builddir}/%{sourcename}/publish/products/ASC.Files/server/* "%{buildroot}%{buildpath}/products/ASC.Files/server/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.Files.Service/service/* "%{buildroot}%{buildpath}/products/ASC.Files/service/"
cp -rf %{_builddir}/%{sourcename}/publish/products/ASC.People/server/* "%{buildroot}%{buildpath}/products/ASC.People/server/"
cp -rf %{_builddir}/%{sourcename}/publish/products/ASC.Projects/server/* "%{buildroot}%{buildpath}/products/ASC.Projects/server/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.ApiSystem/service/* "%{buildroot}%{buildpath}/services/ASC.ApiSystem/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.Data.Backup/service/* "%{buildroot}%{buildpath}/services/ASC.Data.Backup/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.Notify/service/* "%{buildroot}%{buildpath}/services/ASC.Notify/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.Socket.IO/service/* "%{buildroot}%{buildpath}/services/ASC.Socket.IO/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.Socket.IO.Svc/service/* "%{buildroot}%{buildpath}/services/ASC.Socket.IO.Svc/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.SsoAuth/service/* "%{buildroot}%{buildpath}/services/ASC.SsoAuth/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.SsoAuth.Svc/service/* "%{buildroot}%{buildpath}/services/ASC.SsoAuth.Svc/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.Data.Storage.Encryption/service/* "%{buildroot}%{buildpath}/services/ASC.Data.Storage.Encryption/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.Data.Storage.Migration/service/* "%{buildroot}%{buildpath}/services/ASC.Data.Storage.Migration/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.Studio.Notify/service/* "%{buildroot}%{buildpath}/services/ASC.Studio.Notify/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.TelegramService/service/* "%{buildroot}%{buildpath}/services/ASC.TelegramService/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.Thumbnails/service/* "%{buildroot}%{buildpath}/services/ASC.Thumbnails/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.Thumbnails.Svc/service/* "%{buildroot}%{buildpath}/services/ASC.Thumbnails.Svc/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.UrlShortener/service/* "%{buildroot}%{buildpath}/services/ASC.UrlShortener/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.UrlShortener.Svc/service/* "%{buildroot}%{buildpath}/services/ASC.UrlShortener.Svc/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.Web.Api/service/* "%{buildroot}%{buildpath}/studio/api/"
cp -rf %{_builddir}/%{sourcename}/publish/services/ASC.Web.Studio/service/* "%{buildroot}%{buildpath}/studio/server/"
cp -rf %{_builddir}/%{sourcename}/build/install/common/systemd/modules/* "%{buildroot}/lib/systemd/system/"
cp -rf %{_builddir}/%{sourcename}/build/install/common/%{product}-configuration.sh "%{buildroot}%{_bindir}/"
cp -rf %{_builddir}/%{sourcename}/config/* "%{buildroot}%{_sysconfdir}/onlyoffice/%{product}/"
cp -rf %{_builddir}/%{sourcename}/config/nginx/includes/onlyoffice*.conf "%{buildroot}%{_sysconfdir}/nginx/includes/"
cp -rf %{_builddir}/%{sourcename}/config/nginx/onlyoffice*.conf "%{buildroot}%{_sysconfdir}/nginx/conf.d/"
cp -rf %{_builddir}/%{sourcename}/products/ASC.CRM/Client/dist/* "%{buildroot}%{buildpath}/products/ASC.CRM/client/"
cp -rf %{_builddir}/%{sourcename}/products/ASC.Files/Client/dist/* "%{buildroot}%{buildpath}/products/ASC.Files/client/"
cp -rf %{_builddir}/%{sourcename}/products/ASC.Files/Server/DocStore/* "%{buildroot}%{buildpath}/products/ASC.Files/server/DocStore/"
cp -rf %{_builddir}/%{sourcename}/products/ASC.People/Client/dist/* "%{buildroot}%{buildpath}/products/ASC.People/client/"
cp -rf %{_builddir}/%{sourcename}/products/ASC.Projects/Client/dist/* "%{buildroot}%{buildpath}/products/ASC.Projects/client/"
cp -rf %{_builddir}/%{sourcename}/products/ASC.Calendar/Client/dist/* "%{buildroot}%{buildpath}/products/ASC.Calendar/client/"
cp -rf %{_builddir}/%{sourcename}/products/ASC.Mail/Client/dist/* "%{buildroot}%{buildpath}/products/ASC.Mail/client/"
cp -rf %{_builddir}/%{sourcename}/public/* "%{buildroot}%{buildpath}/public/"
cp -rf %{_builddir}/%{sourcename}/web/ASC.Web.Client/dist/* "%{buildroot}%{buildpath}/studio/client/"
cp -rf %{_builddir}/%{sourcename}/web/ASC.Web.Editor/dist/* "%{buildroot}%{buildpath}/products/ASC.Files/editor/"
cp -rf %{_builddir}/%{sourcename}/web/ASC.Web.Login/dist/* "%{buildroot}%{buildpath}/studio/login/"
