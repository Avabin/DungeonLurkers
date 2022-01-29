param (
    [ValidateSet("PierogiesBot", "TheDungeonGuide", "Identity", "All")]
    $Project = "All"
)

if($Project -eq "All") {
    $projects = @("PierogiesBot", "TheDungeonGuide", "Identity");
} else {
    $projects = @($Project);
}

$solutionPath = $PSScriptRoot

foreach ($project in $projects) {
    Write-Host "Running tests for $project"
    $projectBuildScriptPath = (Get-ChildItem -Path $solutionPath -Recurse -Filter "$project.test.ps1").FullName
    ib -File $projectBuildScriptPath -SolutionPath $solutionPath
}