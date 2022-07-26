@echo "MIGRATIONS"
@echo off

PUSHD %~dp0..\common\Tools\Migration.Creator
dotnet run --project Migration.Creator.csproj