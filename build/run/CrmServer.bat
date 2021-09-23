@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\products\ASC.CRM\server\
set servicename=ASC.CRM

PUSHD %~dp0..\..
set servicesource=%cd%\products\ASC.CRM\Server\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5021 $STORAGE_ROOT=..\..\..\..\..\Data log:dir=..\..\..\..\..\Logs log:name=crm pathToConf=..\..\..\..\..\config core:products:folder=..\..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)