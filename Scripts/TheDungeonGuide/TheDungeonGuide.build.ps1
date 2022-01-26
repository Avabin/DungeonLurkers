param (
    $Configuration = "Debug",
    $SolutionDirectory = (Join-Path $PSScriptRoot "..\..\"),
    $SolutionPath = (Get-ChildItem $SolutionDirectory -Filter "*.sln").Directory,
    $CharactersCsProj = (Get-ChildItem $SolutionPath -Recurse -Filter "*.csproj" | Where-Object {$_.BaseName -Contains "TheDungeonGuide.Characters.Host"}),
    $SessionsCsProj = (Get-ChildItem $SolutionPath -Recurse -Filter "*.csproj" | Where-Object {$_.BaseName -Contains "TheDungeonGuide.Sessions.Host"})
)

$projectName = "TheDungeonGuide"
$buildConfiguration = "$Configuration $projectName"

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

task . CleanCharacters, CleanSessions, BuildCharacters, BuildSessions