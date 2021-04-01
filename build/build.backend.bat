PUSHD %~dp0..
dotnet build ASC.Web.sln  /fl1 /flp1:LogFile=build/ASC.Web.log;Verbosity=Normal

echo "ASC.UrlShortener"
call build\scripts\urlshortener.sh

echo "ASC.Thumbnails"
call build\scripts\thumbnails.sh

echo "ASC.Socket.IO"
call build\scripts\socket.sh