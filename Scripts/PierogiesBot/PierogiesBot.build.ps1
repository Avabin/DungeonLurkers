param (
    $Configuration = "Debug",
    $SolutionFile = (Get-ChildItem -Path $SolutionPath -Filter *.sln -Recurse),
    $SolutionPath = $SolutionFile.Directory,
    $CsProj = (Get-ChildItem $SolutionPath -Recurse -Filter "*.csproj" | Where-Object {$_.BaseName -Contains "PierogiesBot.Host"})
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
    exec {dotnet test $SolutionFile.FullName --no-build --filter "BotCrontabRule" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\BotCrontabRules.cobertura.xml" /p:CoverletOutputFormat=cobertura}
}

task TestBotMessageSubscriptions {
    exec {dotnet test $SolutionFile.FullName --no-build --filter "BotMessageSubscription" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\BotMessageSubscriptions.cobertura.xml" /p:CoverletOutputFormat=cobertura}
}

task TestBotReactRules {
    exec {dotnet test $SolutionFile.FullName --no-build --filter "BotReactRule" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\BotReactRules.cobertura.xml" /p:CoverletOutputFormat=cobertura}
}

task TestBotResponseRules {
    exec {dotnet test $SolutionFile.FullName --no-build --filter "BotResponseRule" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\BotResponseRules.cobertura.xml" /p:CoverletOutputFormat=cobertura}
}

task TestGuildSettings {
    exec {dotnet test $SolutionFile.FullName --no-build --filter "GuildSettings" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\GuildSettings.cobertura.xml" /p:CoverletOutputFormat=cobertura}
}

task RunUnitTests {
    exec {dotnet test $SolutionFile.FullName --no-build --filter "cat=Unit" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\PierogiesBotUnit.cobertura.xml" /p:CoverletOutputFormat=cobertura}
}

task Build Clean, BuildHost
task Test TestBotCrontabRules, TestBotMessageSubscriptions, TestBotReactRules, TestBotResponseRules, TestGuildSettings, RunUnitTests

task . Build, Test