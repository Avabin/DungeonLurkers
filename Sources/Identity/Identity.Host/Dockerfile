FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sources/Identity/Identity.Host/Identity.Host.csproj", "Sources/Identity/Identity.Host/"]
COPY ["Sources/Identity/Modules/Identity.Infrastructure/Identity.Infrastructure.csproj", "Sources/Identity/Modules/Identity.Infrastructure/"]
COPY ["Sources/Shared/Persistence/Shared.Persistence.Identity/Shared.Persistence.Identity.csproj", "Sources/Shared/Persistence/Shared.Persistence.Identity/"]
COPY ["Sources/Shared/Persistence/Shared.Persistence.Core/Shared.Persistence.Core.csproj", "Sources/Shared/Persistence/Shared.Persistence.Core/"]
COPY ["Sources/Shared/Shared/Shared.csproj", "Sources/Shared/Shared/"]
COPY ["Sources/Shared/Persistence/Shared.Persistence.Mongo/Shared.Persistence.Mongo.csproj", "Sources/Shared/Persistence/Shared.Persistence.Mongo/"]
COPY ["Sources/Shared/Shared.Infrastructure/Shared.Infrastructure.csproj", "Sources/Shared/Shared.Infrastructure/"]
RUN dotnet restore "Sources/Identity/Identity.Host/Identity.Host.csproj"
COPY . .
WORKDIR "/src/Sources/Identity/Identity.Host"
RUN dotnet build "Identity.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Host.dll"]
