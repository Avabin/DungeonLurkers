param (
    $SolutionPath = (Join-Path $PSScriptRoot "..\..\"),
    $SolutionFile = (Get-ChildItem -Path $SolutionPath -Filter *.sln -Recurse).FullName  
)
Write-Host "Solution file: $SolutionFile"
task TestBotCrontabRules {
    exec {dotnet test $SolutionFile --no-build --filter "BotCrontabRule" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\BotCrontabRules.opencover.xml" /p:CoverletOutputFormat=opencover}
}

task TestBotMessageSubscriptions {
    exec {dotnet test $SolutionFile --no-build --filter "BotMessageSubscription" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\BotMessageSubscriptions.opencover.xml" /p:CoverletOutputFormat=opencover}
}

task TestBotReactRules {
    exec {dotnet test $SolutionFile --no-build --filter "BotReactRule" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\BotReactRules.opencover.xml" /p:CoverletOutputFormat=opencover}
}

task TestBotResponseRules {
    exec {dotnet test $SolutionFile --no-build --filter "BotResponseRule" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\BotResponseRules.opencover.xml" /p:CoverletOutputFormat=opencover}
}

task TestGuildSettings {
    exec {dotnet test $SolutionFile --no-build --filter "GuildSettings" /p:CollectCoverage=true /p:CoverletOutput="$SolutionPath\TestResults\GuildSettings.opencover.xml" /p:CoverletOutputFormat=opencover}
}

task .  TestBotCrontabRules, TestBotMessageSubscriptions, TestBotReactRules, TestBotResponseRules, TestGuildSettings