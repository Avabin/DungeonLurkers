FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sources/TheDungeonGuide/TheDungeonGuide.Characters.Host/TheDungeonGuide.Characters.Host.csproj", "Sources/TheDungeonGuide/TheDungeonGuide.Characters.Host/"]
COPY ["Sources/Shared/Persistence/Shared.Persistence.Mongo/Shared.Persistence.Mongo.csproj", "Sources/Shared/Persistence/Shared.Persistence.Mongo/"]
COPY ["Sources/Shared/Persistence/Shared.Persistence.Core/Shared.Persistence.Core.csproj", "Sources/Shared/Persistence/Shared.Persistence.Core/"]
COPY ["Sources/Shared/Shared/Shared.csproj", "Sources/Shared/Shared/"]
COPY ["Sources/Shared/Shared.Infrastructure/Shared.Infrastructure.csproj", "Sources/Shared/Shared.Infrastructure/"]
COPY ["Sources/TheDungeonGuide/Persistence/Modules/TheDungeonGuide.Persistence.Characters/TheDungeonGuide.Persistence.Characters.csproj", "Sources/TheDungeonGuide/Persistence/Modules/TheDungeonGuide.Persistence.Characters/"]
COPY ["Sources/Shared/TheDungeonGuide/TheDungeonGuide.Shared/TheDungeonGuide.Shared.csproj", "Sources/Shared/TheDungeonGuide/TheDungeonGuide.Shared/"]
RUN dotnet restore "Sources/TheDungeonGuide/TheDungeonGuide.Characters.Host/TheDungeonGuide.Characters.Host.csproj"
COPY . .
WORKDIR "/src/Sources/TheDungeonGuide/TheDungeonGuide.Characters.Host"
RUN dotnet build "TheDungeonGuide.Characters.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TheDungeonGuide.Characters.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TheDungeonGuide.Characters.Host.dll"]
