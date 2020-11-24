PUSHD %~dp0
call start\stop.bat

PUSHD %~dp0..

echo "Build FRONT-END"
call yarn install

echo "ASC.UrlShortener"
call yarn install --cwd common/ASC.UrlShortener > build\ASC.UrlShortener.log

echo "ASC.Web.sln"
call dotnet build ASC.Web.sln  /fl1 /flp1:LogFile=build/ASC.Web.log;Verbosity=Normal

start /b call build\start\start.bat

pause