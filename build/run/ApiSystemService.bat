@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\apisystem
set servicename=ASC.ApiSystem

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.ApiSystem\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe  --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=apisystem
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)