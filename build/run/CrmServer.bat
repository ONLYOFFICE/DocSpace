@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\products\ASC.CRM\server\
set servicename=ASC.CRM

PUSHD %~dp0..\..
set servicesource=%cd%\products\ASC.CRM\Server\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe  --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=crm
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)