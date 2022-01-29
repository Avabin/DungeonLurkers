param (
    $Configuration = "Debug",
    $SolutionPath = (Get-ChildItem $PSScriptRoot -Filter "*.sln").Directory,
    $CsProj = (Get-ChildItem $SolutionPath -Recurse -Filter "*.csproj" | Where-Object {$_.BaseName -Contains "Identity.Host"})
)

$projectName = "PierogiesBot"
$buildConfiguration = "$Configuration $projectName"

task Clean {
    Write-Host "Cleaning $projectName at $($CsProj.FullName)"
    exec {dotnet clean $CsProj.FullName}
}

task Build {
    Write-Host "Building $projectName at $($CsProj.FullName)"
    exec {dotnet build $CsProj.FullName --configuration $buildConfiguration}
}

task . Clean, Build