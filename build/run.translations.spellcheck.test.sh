rd="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "Run script directory:" $dir

dir=$(builtin cd $rd/../; pwd)

save=false

while getopts s: flag
do
    case "${flag}" in
        s) save=${OPTARG};;
    esac
done

echo "Root directory:" $dir

dotnet test $dir/common/Tests/Frontend.Translations.Tests/Frontend.Translations.Tests.csproj --filter Name~SpellCheckTest -l:html --results-directory "$dir/TestsResults" --environment "BASE_DIR=$dir" --environment "SAVE=$save"