@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\common\services\ASC.UrlShortener.Svc\bin\Debug\ASC.UrlShortener.Svc.exe urls=http://0.0.0.0:5029 $STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=urlshortener pathToConf=%cd%\config core:products:folder=%cd%\products core:products:subfolder=server