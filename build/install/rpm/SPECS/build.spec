%build

bash build/install/common/systemd/build.sh

bash build/install/common/build-frontend.sh --srcpath %{_builddir}/AppServer-%GIT_BRANCH
bash build/install/common/build-backend.sh --srcpath %{_builddir}/AppServer-%GIT_BRANCH
bash build/install/common/publish-backend.sh --srcpath %{_builddir}/AppServer-%GIT_BRANCH --arguments "--disable-parallel"

sed -i "s@var/www@var/www/appserver@" config/nginx/onlyoffice-*.conf && sed -i "s@var/www@var/www/appserver@" config/nginx/includes/onlyoffice-*.conf
