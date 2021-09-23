@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\products\ASC.People\server\
set servicename=ASC.People

PUSHD %~dp0..\..
set servicesource=%cd%\products\ASC.People\Server\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5004 $STORAGE_ROOT=..\..\..\..\..\Data log:dir=..\..\..\..\..\Logs log:name=people pathToConf=..\..\..\..\..\config core:products:folder=..\..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)