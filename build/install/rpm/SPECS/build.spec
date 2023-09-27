%build

bash build/install/common/systemd/build.sh

bash build/install/common/build-frontend.sh --srcpath %{_builddir}/%{sourcename}
bash build/install/common/build-backend.sh --srcpath %{_builddir}/%{sourcename}
bash build/install/common/publish-backend.sh --srcpath %{_builddir}/%{sourcename} 
rename -f -v "s/product([^\/]*)$/%{product}\$1/g" build/install/common/*
sed -i "s/{{product}}/%{product}/g" %{_builddir}/%{sourcename}/build/install/common/logrotate/product-common

rm -f %{_builddir}/%{sourcename}/config/nginx/onlyoffice-login.conf

if ! grep -q 'var/www/%{product}' config/nginx/*.conf; then find config/nginx/ -name "*.conf" -exec sed -i "s@\(var/www/\)@\1%{product}/@" {} +; fi

json -I -f %{_builddir}/%{sourcename}/config/appsettings.services.json -e "this.logPath=\"/var/log/onlyoffice/%{product}\"" -e "this.socket={ 'path': '../ASC.Socket.IO/' }" \
-e "this.ssoauth={ 'path': '../ASC.SsoAuth/' }" -e "this.logLevel=\"warning\""  -e "this.core={ 'products': { 'folder': '%{buildpath}/products', 'subfolder': 'server'} }"

find %{_builddir}/%{sourcename}/config/ -type f -regex '.*\.\(test\|dev\).*' -delete
json -I -f %{_builddir}/%{sourcename}/config/appsettings.json -e "this.core.notify.postman=\"services\"" -e "this.Logging.LogLevel.Default=\"Warning\"" -e "this['debug-info'].enabled=\"false\"" -e "this.web.samesite=\"None\""
json -I -f %{_builddir}/%{sourcename}/config/apisystem.json -e "this.core.notify.postman=\"services\""
sed 's_\(minlevel=\)".*"_\1"Warn"_g' -i %{_builddir}/%{sourcename}/config/nlog.config

sed 's_etc/nginx_etc/openresty_g' -i %{_builddir}/%{sourcename}/config/nginx/*.conf
sed 's/teamlab.info/onlyoffice.com/g' -i %{_builddir}/%{sourcename}/config/autofac.consumers.json
sed -e 's/$router_host/127.0.0.1/g' -e 's/the_host/host/g' -e 's/the_scheme/scheme/g' -e 's_includes_/etc/openresty/includes_g' -i %{_builddir}/%{sourcename}/build/install/docker/config/nginx/onlyoffice-proxy*.conf
sed -e '/.pid/d' -e '/temp_path/d' -e 's_etc/nginx_etc/openresty_g' -i %{_builddir}/%{sourcename}/build/install/docker/config/nginx/templates/nginx.conf.template
sed -i "s_\(.*root\).*;_\1 \"/var/www/%{product}\";_g" -i %{_builddir}/%{sourcename}/build/install/docker/config/nginx/letsencrypt.conf

find %{_builddir}/%{sourcename}/publish/ \
     %{_builddir}/%{sourcename}/ASC.Migration.Runner \
     -depth -type f -regex '.*\(dll\|dylib\|so\)$' -exec chmod 755 {} \;

find %{_builddir}/%{sourcename}/publish/ \
     %{_builddir}/%{sourcename}/ASC.Migration.Runner \
     -depth -type f -regex '.*\(so\)$' -exec strip {} \;
