echo "MIGRATIONS"

PUSHD %~dp0..\common\Tools\AutoMigrationCreator
dotnet run ASC.Tools.sln

pause