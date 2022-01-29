param (
    $SolutionPath = (Join-Path $PSScriptRoot "..\..\"),
    $SolutionFile = (Get-ChildItem -Path $SolutionPath -Filter *.sln -Recurse).FullName  
)
Write-Host "Solution file: $SolutionFile"
task TestIdentity {
    exec {dotnet test $SolutionFile --no-build --filter "cat=Identity" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\Identity.opencover.xml" /p:CoverletOutputFormat=opencover}
}

task . TestIdentity