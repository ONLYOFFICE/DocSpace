@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\products\ASC.Files\service\
set servicename=ASC.Files.Service

PUSHD %~dp0..\..
set servicesource=%cd%\products\ASC.Files\Service\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5009 $STORAGE_ROOT=..\..\..\..\..\Data log:dir=..\..\..\..\..\Logs log:name=files.service pathToConf=..\..\..\..\..\config core:products:folder=..\..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)