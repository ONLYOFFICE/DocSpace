PUSHD %~dp0..
set dir=%~dp0..
echo %dir%
dotnet test common\Tests\Backend.Translations.Tests\Backend.Translations.Tests.csproj -l:html --environment "BASE_DIR=%dir%" --results-directory "%dir%/TestsResults"