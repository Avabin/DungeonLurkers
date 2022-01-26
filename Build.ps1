param (
    [ValidateSet("Debug", "Release")]
    $Configuration = "Debug",
    [ValidateSet("PierogiesBot", "TheDungeonGuide", "All")]
    $Project = "All"
)

if($Project -eq "All") {
    $projects = @("PierogiesBot", "TheDungeonGuide");
} else {
    $projects = @($Project);
}

$solutionPath = $PSScriptRoot

foreach ($project in $projects) {
    Write-Host "Running build for $project"
    $projectBuildScriptPath = (Get-ChildItem -Path $solutionPath -Recurse -Filter "$project.build.ps1").FullName
    ib -File $projectBuildScriptPath -Configuration $Configuration -SolutionPath $solutionPath
}