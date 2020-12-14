PUSHD %~dp0..

del /f /q web\ASC.Web.Components\yarn.lock
del /f /q web\ASC.Web.Common\yarn.lock
del /f /q web\ASC.Web.Client\yarn.lock
del /f /q products\ASC.Files\Client\yarn.lock
del /f /q products\ASC.People\Client\yarn.lock

start /b call build\rebuild.frontend.bat

