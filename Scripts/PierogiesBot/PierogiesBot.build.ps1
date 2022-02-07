param (
    $Configuration = "Debug",
    $SolutionFile = (Get-ChildItem -Path $SolutionPath -Filter *.sln -Recurse),
    $SolutionPath = $SolutionFile.Directory,
    $CsProj = (Get-ChildItem $SolutionPath -Recurse -Filter "*.csproj" | Where-Object {$_.BaseName -Contains "PierogiesBot.Host"}),
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

Write-Host "Solution file: $SolutionFile"
task TestBotCrontabRules {
    exec {dotnet test $SolutionFile.FullName --no-build --filter "BotCrontabRule" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\BotCrontabRules.opencover.xml" /p:CoverletOutputFormat=opencover}
}

task TestBotMessageSubscriptions {
    exec {dotnet test $SolutionFile.FullName --no-build --filter "BotMessageSubscription" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\BotMessageSubscriptions.opencover.xml" /p:CoverletOutputFormat=opencover}
}

task TestBotReactRules {
    exec {dotnet test $SolutionFile.FullName --no-build --filter "BotReactRule" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\BotReactRules.opencover.xml" /p:CoverletOutputFormat=opencover}
}

task TestBotResponseRules {
    exec {dotnet test $SolutionFile.FullName --no-build --filter "BotResponseRule" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\BotResponseRules.opencover.xml" /p:CoverletOutputFormat=opencover}
}

task TestGuildSettings {
    exec {dotnet test $SolutionFile.FullName --no-build --filter "GuildSettings" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\GuildSettings.opencover.xml" /p:CoverletOutputFormat=opencover}
}

task RunUnitTests {
    exec {dotnet test $SolutionFile.FullName --no-build --filter "cat=Unit" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\PierogiesBotUnit.opencover.xml" /p:CoverletOutputFormat=opencover}
}

task BuildDockerImage {
    Write-Host "Building Docker image for $projectName"
    $dockerfile = Join-Path $ProjectDirectory.FullName "Dockerfile"
    exec {dotnet clean $SolutionFile.FullName}

    exec {docker build -f $dockerfile -t pierogiesbot:latest $SolutionFile.Directory.FullName}
}

task Build Clean, BuildHost
task Test TestBotCrontabRules, TestBotMessageSubscriptions, TestBotReactRules, TestBotResponseRules, TestGuildSettings, RunUnitTests

task . Build, Test