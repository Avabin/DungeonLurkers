FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY DungeonLurkers.sln ./
COPY **/*.csproj **/**/*.csproj **/**/**/*.csproj **/**/**/**/*.csproj **/**/**/**/**/*.csproj ./
RUN dotnet sln list | while read line ; do mkdir -pv $(dirname $line) && mv $(basename $line) $line;done
RUN dotnet restore "Sources/Identity/Identity.Host/Identity.Host.csproj"

FROM build AS publish
COPY . .
WORKDIR Sources
RUN dotnet publish "Identity/Identity.Host/Identity.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Host.dll"]
