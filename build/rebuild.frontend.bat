PUSHD %~dp0
call start\stop.bat

PUSHD %~dp0..

del /s /q packages\asc-web-components
del /s /q packages\asc-web-common

echo "ASC.Web.Components"
call yarn install --cwd web/ASC.Web.Components > build\ASC.Web.Components.log
REM xcopy web\ASC.Web.Components\node_modules packages\asc-web-components\node_modules\ /E /R /Y >> build\ASC.Web.Components.log
call yarn install --cwd packages/asc-web-components >> build\ASC.Web.Components.log
call yarn link --cwd packages/asc-web-components

echo "ASC.Web.Common"
call yarn link "asc-web-components" --cwd web/ASC.Web.Common
call yarn install --cwd web/ASC.Web.Common > build\ASC.Web.Common.log
REM xcopy web\ASC.Web.Common\node_modules packages\asc-web-common\node_modules\ /E /R /Y >> build\ASC.Web.Components.log
call yarn install --cwd packages/asc-web-common >> build\ASC.Web.Common.log
call yarn link --cwd packages/asc-web-common

echo "ASC.Web.Client"
call yarn link "asc-web-components" --cwd web/ASC.Web.Client
call yarn link "asc-web-common" --cwd web/ASC.Web.Client
call yarn install --cwd web/ASC.Web.Client > build\ASC.Web.Client.log

echo "ASC.Web.People.Client"
call yarn link "asc-web-components" --cwd products/ASC.People/Client
call yarn link "asc-web-common" --cwd products/ASC.People/Client
call yarn install --cwd products/ASC.People/Client > build\ASC.Web.People.Client.log

echo "ASC.Web.Files.Client"
call yarn link "asc-web-components" --cwd products/ASC.Files/Client
call yarn link "asc-web-common" --cwd products/ASC.Files/Client
call yarn install --cwd products/ASC.Files/Client > build\ASC.Web.Files.Client.log

start /b call build\start\start.bat

pause