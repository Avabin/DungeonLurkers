param (
    [ValidateSet("Debug", "Release", "Debug TheDungeonGuide", "Release TheDungeonGuide")]
    $Configuration = "Debug",
    $SolutionDirectory = (Join-Path $PSScriptRoot "..\..\"),
    $SolutionFile = (Get-ChildItem $SolutionDirectory -Filter "*.sln"),
    $SolutionPath = $SolutionFile.Directory,
    $CharactersCsProj = (Get-ChildItem $SolutionPath -Recurse -Filter "*.csproj" | Where-Object {$_.BaseName -Contains "TheDungeonGuide.Characters.Host"}),
    $SessionsCsProj = (Get-ChildItem $SolutionPath -Recurse -Filter "*.csproj" | Where-Object {$_.BaseName -Contains "TheDungeonGuide.Sessions.Host"})
)

$projectName = "TheDungeonGuide"
$buildConfiguration = "$Configuration $projectName"
Write-Host "Solution file: $SolutionFile"

task CleanCharacters {
    Write-Host "Cleaning $CharactersCsProj"
    exec {dotnet clean $CharactersCsProj.FullName}
}

task CleanSessions {
    Write-Host "Cleaning $SessionsCsProj"
    exec {dotnet clean $SessionsCsProj.FullName}
}

task BuildCharacters {
    Write-Host "Building $CharactersCsProj"
    exec {dotnet build $CharactersCsProj.FullName -c $buildConfiguration}
}

task BuildSessions {
    Write-Host "Building $SessionsCsProj"
    exec {dotnet build $SessionsCsProj.FullName -c $buildConfiguration}
}

task TestCharacters {
    exec { dotnet test $SolutionFile.FullName --no-build --filter "Characters" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\Characters.opencover.xml" /p:CoverletOutputFormat=opencover }
}

task TestSessions {
    exec { dotnet test $SolutionFile.FullName --no-build --filter "Sessions" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\Sessions.opencover.xml" /p:CoverletOutputFormat=opencover }
}

task BuildDockerImage {
    Write-Host "Building Docker image for $projectName"
    $dockerfile = Join-Path $SessionsCsProj.FullName "Dockerfile"
    exec {dotnet clean $SolutionFile.FullName}

    Write-Host "Building Docker image for sessions host"

    $dockerfile = Join-Path $SessionsCsProj.Directory.FullName "Dockerfile"
    exec {docker build -f $dockerfile -t tdg-sessions:latest $SolutionFile.Directory.FullName}

    Write-Host "Building Docker image for characters host"

    $dockerfile = Join-Path $CharactersCsProj.Directory.FullName "Dockerfile"
    exec {docker build -f $dockerfile -t tdg-characters:latest $SolutionFile.Directory.FullName}
}

task Build CleanCharacters, CleanSessions, BuildCharacters, BuildSessions
task Test TestCharacters, TestSessions

task . Build, Test