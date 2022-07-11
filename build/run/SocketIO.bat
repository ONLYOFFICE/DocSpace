@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\common\services\ASC.Socket.IO.Svc\bin\Debug\ASC.Socket.IO.Svc.exe urls=http://0.0.0.0:5028 $STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=socketio pathToConf=%cd%\config core:products:folder=%cd%\products core:products:subfolder=server socket:path=%cd%\common\ASC.Socket.IO