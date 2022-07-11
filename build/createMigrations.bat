@echo "MIGRATIONS"
@echo off

PUSHD %~dp0..\common\Tools\AutoMigrationCreator
dotnet run --project AutoMigrationCreator.csproj