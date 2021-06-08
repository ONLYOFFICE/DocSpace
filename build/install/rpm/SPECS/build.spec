%build

bash build/install/common/systemd/build.sh

bash build/install/common/build-frontend.sh --srcpath %{_builddir}/%{sourcename}
bash build/install/common/build-backend.sh --srcpath %{_builddir}/%{sourcename}
bash build/install/common/publish-backend.sh --srcpath %{_builddir}/%{sourcename} --arguments "--disable-parallel"

sed -i "s@var/www@var/www/%{product}@" config/nginx/onlyoffice-*.conf && sed -i "s@var/www@var/www/%{product}@" config/nginx/includes/onlyoffice-*.conf
