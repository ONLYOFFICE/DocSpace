@echo "MIGRATIONS"
@echo off

PUSHD %~dp0..\common\Tools\Migration.Runner
dotnet run --project Migration.Runner.csproj
dotnet run --project Migration.Runner.csproj --options:Path=C:\Git\portals_core\common\ASC.Data.Backup.Core\bin\Debug\net6.0