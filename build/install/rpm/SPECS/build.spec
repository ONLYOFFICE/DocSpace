%build

bash build/install/common/systemd/build.sh

bash build/install/common/build-frontend.sh --srcpath %{_builddir}/%{sourcename}
bash build/install/common/build-backend.sh --srcpath %{_builddir}/%{sourcename}
bash build/install/common/publish-backend.sh --srcpath %{_builddir}/%{sourcename} -ar "-r linux-x64"
rename -f -v "s/product([^\/]*)$/%{product}\$1/g" build/install/common/*
sed -i "s/{{product}}/%{product}/g" %{_builddir}/%{sourcename}/build/install/common/logrotate/product-common

rm -rf %{_builddir}/%{sourcename}/ASC.Migration.Runner/service/runtimes/linux-arm*
rm -f %{_builddir}/%{sourcename}/config/nginx/onlyoffice-login.conf
sed -i "s@var/www@var/www/%{product}@g" config/nginx/*.conf && sed -i "s@var/www@var/www/%{product}@g" config/nginx/includes/*.conf

json -I -f %{_builddir}/%{sourcename}/config/appsettings.services.json -e "this.logPath=\"/var/log/onlyoffice/%{product}\"" -e "this.socket={ 'path': '../ASC.Socket.IO/' }" \
-e "this.ssoauth={ 'path': '../ASC.SsoAuth/' }" -e "this.core={ 'products': { 'folder': '%{buildpath}/products', 'subfolder': 'server'} }"

find %{_builddir}/%{sourcename}/publish/ \
     %{_builddir}/%{sourcename}/ASC.Migration.Runner \
     -depth -type f -regex '.*\(dll\|dylib\|so\)$' -exec chmod 755 {} \;

find %{_builddir}/%{sourcename}/publish/ \
     %{_builddir}/%{sourcename}/ASC.Migration.Runner \
     -depth -type f -regex '.*\(so\)$' -exec strip {} \;
