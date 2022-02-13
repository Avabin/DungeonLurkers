param (
    $SolutionPath,
    $Configuration = "Debug",
    $SolutionFile = (Get-ChildItem -Path $SolutionPath -Filter *.sln -Recurse),
    $CsProj = (Get-ChildItem $SolutionPath -Recurse -Filter "*.csproj" | Where-Object {$_.BaseName -Contains "Identity.Host"}),
    $ProjectDirectory = $CsProj.Directory
)
$projectName = "Identity"
$buildConfiguration = "$Configuration $projectName"

task Clean {
    Write-Host "Cleaning $projectName at $($CsProj.FullName)"
    exec {dotnet clean -c $buildConfiguration $CsProj.FullName }
}

task BuildHost {
    Write-Host "Building $projectName at $($CsProj.FullName)"
    exec {dotnet build --configuration $buildConfiguration $CsProj.FullName }
}


task TestIdentity {
    exec {dotnet test $SolutionFile.FullName --no-build --filter "cat=Identity" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\Identity.opencover.xml" /p:CoverletOutputFormat=opencover}
}

task BuildDockerImage {
    Write-Host "Building Docker image for $projectName"
    $dockerfile = Join-Path $ProjectDirectory.FullName "Dockerfile"
    exec {dotnet clean -c $buildConfiguration $SolutionFile.FullName}
    $dockerContext = $SolutionPath
    Write-Host "Docker context: $dockerContext"
    exec {docker build -f $dockerfile -t ghcr.io/avabin/identity:latest $dockerContext}
}

task Test TestIdentity
task Build Clean, BuildHost