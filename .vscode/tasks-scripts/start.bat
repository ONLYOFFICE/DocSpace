@echo off

set root=%~dp0..\..

@REM if "%2%"=="Local" goto onLocal
@REM if "%2%"=="Docker" goto onDocker


@REM :onLocal
if "%1%"=="Start" goto onStartLocal
if "%1%"=="Restart" goto onRestartLocal
if "%1%"=="Stop" goto onStopLocal

:onStartLocal
"%root%\\build\\start\start.bat" start

:onRestartLocal
"%root%\\build\\start\\restart.bat" start

:onStopLocal
"%root%\\build\\start\\stop.bat" start 


@REM :onDocker
@REM if "%1%"=="Start" goto onStartDocker
@REM if "%1%"=="Restart" goto onRestartDocker
@REM if "%1%"=="Stop" goto onStopDocker

@REM :onStartDocker
@REM echo "Start docker"
@REM "%root%\\build\\start\start.backend.docker.bat" start

@REM :onRestartDocker
@REM "%root%\\build\\start\\restart.backend.docker.bat" start

@REM :onStopDocker
@REM "%root%\\build\\start\\stop.backend.docker.bat" start 