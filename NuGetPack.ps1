$csproj = (ls *.Sdk\*.csproj).FullName

dotnet pack "$csproj" -o . 
dotnet pack "$csproj" -o . --version-suffix "develop"

$LASTEXITCODE = 0