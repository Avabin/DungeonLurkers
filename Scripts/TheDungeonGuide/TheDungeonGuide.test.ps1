param (
    $SolutionPath = (Join-Path $PSScriptRoot "..\..\"),
    $SolutionFile = (Get-ChildItem -Path $SolutionPath -Filter *.sln -Recurse).FullName
)
Write-Host "Solution file: $SolutionFile"
task TestCharacters {
    exec { dotnet test $SolutionFile --no-build --filter "Characters" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\Characters.opencover.xml" /p:CoverletOutputFormat=opencover }
}

task TestSessions {
    exec { dotnet test $SolutionFile --no-build --filter "Sessions" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\Sessions.opencover.xml" /p:CoverletOutputFormat=opencover }
}

task .  TestCharacters, TestSessions