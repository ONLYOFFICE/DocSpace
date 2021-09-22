@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\backup
set servicename=ASC.Data.Backup

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.Data.Backup\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe  --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=backup
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)