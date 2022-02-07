FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sources/PierogiesBot/PierogiesBot.Host/PierogiesBot.Host.csproj", "Sources/PierogiesBot/PierogiesBot.Host/"]
COPY ["Sources/Shared/Persistence/Shared.Persistence.Mongo/Shared.Persistence.Mongo.csproj", "Sources/Shared/Persistence/Shared.Persistence.Mongo/"]
COPY ["Sources/Shared/Persistence/Shared.Persistence.Core/Shared.Persistence.Core.csproj", "Sources/Shared/Persistence/Shared.Persistence.Core/"]
COPY ["Sources/Shared/Shared/Shared.csproj", "Sources/Shared/Shared/"]
COPY ["Sources/Shared/Shared.Infrastructure/Shared.Infrastructure.csproj", "Sources/Shared/Shared.Infrastructure/"]
COPY ["Sources/PierogiesBot/Discord/Modules/PierogiesBot.Discord.Infrastructure/PierogiesBot.Discord.Infrastructure.csproj", "Sources/PierogiesBot/Discord/Modules/PierogiesBot.Discord.Infrastructure/"]
COPY ["Sources/PierogiesBot/Persistence/PierogiesBot.Persistence.BotCrontabRule/PierogiesBot.Persistence.BotCrontabRule.csproj", "Sources/PierogiesBot/Persistence/PierogiesBot.Persistence.BotCrontabRule/"]
COPY ["Sources/Shared/PierogiesBot/PierogiesBot.Shared/PierogiesBot.Shared.csproj", "Sources/Shared/PierogiesBot/PierogiesBot.Shared/"]
COPY ["Sources/PierogiesBot/Discord/Modules/PierogiesBot.Discord.Commands/PierogiesBot.Discord.Commands.csproj", "Sources/PierogiesBot/Discord/Modules/PierogiesBot.Discord.Commands/"]
COPY ["Sources/PierogiesBot/Persistence/PierogiesBot.Persistence.BotMessageSubscription/PierogiesBot.Persistence.BotMessageSubscription.csproj", "Sources/PierogiesBot/Persistence/PierogiesBot.Persistence.BotMessageSubscription/"]
COPY ["Sources/PierogiesBot/Persistence/PierogiesBot.Persistence.BotReactRules/PierogiesBot.Persistence.BotReactRules.csproj", "Sources/PierogiesBot/Persistence/PierogiesBot.Persistence.BotReactRules/"]
COPY ["Sources/PierogiesBot/Persistence/PierogiesBot.Persistence.Shared/PierogiesBot.Persistence.Shared.csproj", "Sources/PierogiesBot/Persistence/PierogiesBot.Persistence.Shared/"]
COPY ["Sources/PierogiesBot/Persistence/PierogiesBot.Persistence.BotResponseRules/PierogiesBot.Persistence.BotResponseRules.csproj", "Sources/PierogiesBot/Persistence/PierogiesBot.Persistence.BotResponseRules/"]
COPY ["Sources/PierogiesBot/Persistence/PierogiesBot.Persistence.GuildSettings/PierogiesBot.Persistence.GuildSettings.csproj", "Sources/PierogiesBot/Persistence/PierogiesBot.Persistence.GuildSettings/"]
COPY ["Sources/PierogiesBot/Discord/Modules/PierogiesBot.Discord.Core/PierogiesBot.Discord.Core.csproj", "Sources/PierogiesBot/Discord/Modules/PierogiesBot.Discord.Core/"]
RUN dotnet restore "Sources/PierogiesBot/PierogiesBot.Host/PierogiesBot.Host.csproj"
COPY . .
WORKDIR "/src/Sources/PierogiesBot/PierogiesBot.Host"
RUN dotnet build "PierogiesBot.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PierogiesBot.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PierogiesBot.Host.dll"]
