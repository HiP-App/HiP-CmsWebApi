$csproj = (ls *.Sdk\*.csproj).FullName

dotnet pack "$csproj" -o .

$nupkg = (ls src\*.Sdk\*.nupkg).FullName
dotnet nuget push "$nupkg" -k "$env:MyGetKey" -s "$env:NuGetFeed"
$LASTEXITCODE = 0