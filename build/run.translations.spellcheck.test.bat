PUSHD %~dp0..
dotnet test common\Tests\Frontend.Translations.Tests\Frontend.Translations.Tests.csproj --filter Name~SpellCheckTest -l:html -r TestsResults