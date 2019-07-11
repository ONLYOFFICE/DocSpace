echo "RUN ASC.People"
PUSHD %~dp0..\..\products\ASC.People\Server\
call dotnet watch run --project ASC.People.csproj