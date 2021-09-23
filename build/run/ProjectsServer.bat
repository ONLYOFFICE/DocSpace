@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\products\ASC.Projects\server\
set servicename=ASC.Projects

PUSHD %~dp0..\..
set servicesource=%cd%\products\ASC.Projects\Server\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5020 $STORAGE_ROOT=..\..\..\..\..\Data log__dir=..\..\..\..\..\Logs log:name=projects pathToConf=..\..\..\..\..\config core:products:folder=..\..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)