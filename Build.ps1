param (
    [ValidateSet("Debug", "Release")]
    $Configuration = "Debug",
    [ValidateSet("PierogiesBot", "TheDungeonGuide", "Identity", "All")]
    $Project = "All",
    [switch]$Docker
)

if($Project -eq "All") {
    $projects = @("PierogiesBot", "TheDungeonGuide", "Identity");
} else {
    $projects = @($Project);
}

$solutionPath = $PSScriptRoot

foreach ($project in $projects) {
    Write-Host "Running build for $project"
    $projectBuildScriptPath = (Get-ChildItem -Path $solutionPath -Recurse -Filter "$project.build.ps1").FullName
    if($Docker) {
        ib -File $projectBuildScriptPath BuildDockerImage -Configuration $Configuration -SolutionPath $solutionPath
    } else {
        ib -File $projectBuildScriptPath Build -Configuration $Configuration -SolutionPath $solutionPath
    }
}