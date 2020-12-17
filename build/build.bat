PUSHD %~dp0
call runasadmin.bat "%~dpnx0"
if %errorlevel% == 0 (

call start\stop.bat

PUSHD %~dp0..

echo "ASC.Web.Components"
call build\scripts\components.sh

echo "ASC.Web.Common"
call build\scripts\common.sh

echo "ASC.Web.Client"
call build\scripts\client.sh

echo "ASC.Web.People.Client"
call build\scripts\people.sh

echo "ASC.Web.Files.Client"
call build\scripts\files.sh

echo "ASC.UrlShortener"
call build\scripts\urlshortener.sh

echo "ASC.Thumbnails"
call build\scripts\thumbnails.sh

echo "ASC.Socket.IO"
call build\scripts\socket.sh

echo "ASC.Web.sln"
call build\build.sln.bat

start /b call build\start\start.bat

pause
)