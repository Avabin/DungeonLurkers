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

Remove-Item .\TestResults\* -Recurse -Force -ErrorAction SilentlyContinue

foreach ($project in $projects) {
    Write-Host "Running tests for $project"
    $projectBuildScriptPath = (Get-ChildItem -Path $solutionPath -Recurse -Filter "$project.build.ps1").FullName
    ib Test -File $projectBuildScriptPath -SolutionPath $solutionPath
}