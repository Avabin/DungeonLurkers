FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY DungeonLurkers.sln ./
COPY **/*.csproj **/**/*.csproj **/**/**/*.csproj **/**/**/**/*.csproj **/**/**/**/**/*.csproj ./
RUN dotnet sln list | while read line ; do mkdir -pv $(dirname $line) && mv $(basename $line) $line;done
RUN dotnet restore "Sources/TheDungeonGuide/TheDungeonGuide.Characters.Host/TheDungeonGuide.Characters.Host.csproj"
COPY Sources/ Sources/
WORKDIR "Sources/TheDungeonGuide/TheDungeonGuide.Characters.Host"

FROM build AS publish
RUN dotnet publish "TheDungeonGuide.Characters.Host.csproj" -c "Release TheDungeonGuide" -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TheDungeonGuide.Characters.Host.dll"]
