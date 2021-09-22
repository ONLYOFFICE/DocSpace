@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\products\ASC.Projects\server
set servicename=ASC.Projects

PUSHD %~dp0..\..
set servicesource=%cd%\products\ASC.Projects\Server\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe  --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=projects
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)