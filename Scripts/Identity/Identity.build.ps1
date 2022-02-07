param (
    $SolutionPath,
    $Configuration = "Debug",
    $SolutionFile = (Get-ChildItem -Path $SolutionPath -Filter *.sln -Recurse),
    $CsProj = (Get-ChildItem $SolutionPath -Recurse -Filter "*.csproj" | Where-Object {$_.BaseName -Contains "Identity.Host"}),
    $ProjectDirectory = $CsProj.Directory
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


task TestIdentity {
    exec {dotnet test $SolutionFile.FullName --no-build --filter "cat=Identity" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\Identity.opencover.xml" /p:CoverletOutputFormat=opencover}
}

task BuildDockerImage {
    Write-Host "Building Docker image for $projectName"
    $dockerfile = Join-Path $ProjectDirectory.FullName "Dockerfile"
    exec {dotnet clean $SolutionFile.FullName}

    exec {docker build -f $dockerfile -t identity:latest $SolutionFile.Directory.FullName}
}

task Test TestIdentity
task Build Clean, BuildHost