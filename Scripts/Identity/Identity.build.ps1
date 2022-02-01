param (
    $Configuration = "Debug",
    $SolutionFile = (Get-ChildItem -Path $SolutionPath -Filter *.sln -Recurse),
    $SolutionPath = $SolutionFile.Directory,
    $CsProj = (Get-ChildItem $SolutionPath -Recurse -Filter "*.csproj" | Where-Object {$_.BaseName -Contains "Identity.Host"})
)

$projectName = "PierogiesBot"
$buildConfiguration = "$Configuration $projectName"

task Clean {
    Write-Host "Cleaning $projectName at $($CsProj.FullName)"
    exec {dotnet clean $CsProj.FullName}
}

task BuildHost {
    Write-Host "Building $projectName at $($CsProj.FullName)"
    exec {dotnet build $CsProj.FullName --configuration $buildConfiguration}
}

Write-Host "Solution file: $SolutionFile"
task TestIdentity {
    exec {dotnet test $SolutionFile.FullName --no-build --filter "cat=Identity" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\Identity.cobertura.xml" /p:CoverletOutputFormat=cobertura}
}

task Test TestIdentity
task Build Clean, BuildHost

task . Clean, Build