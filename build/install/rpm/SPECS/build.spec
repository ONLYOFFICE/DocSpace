%build

bash build/install/common/systemd/build.sh

dotnet restore ASC.Web.sln --configfile .nuget/NuGet.Config --disable-parallel
bash build/install/common/build-backend.sh --srcpath %{_builddir}/AppServer-%GIT_BRANCH
bash build/install/common/build-frontend.sh --srcpath %{_builddir}/AppServer-%GIT_BRANCH
bash build/install/common/publish-backend.sh --srcpath %{_builddir}/AppServer-%GIT_BRANCH --buildpath  %{_builddir}

sed -i "s@var/www@var/www/appserver@" config/nginx/onlyoffice-*.conf
