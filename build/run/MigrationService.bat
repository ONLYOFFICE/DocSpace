@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\migration
set servicename=ASC.Data.Storage.Migration

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.Data.Storage.Migration\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=migration
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)