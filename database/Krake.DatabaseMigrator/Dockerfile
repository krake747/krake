FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["database/Krake.DatabaseMigrator/Krake.DatabaseMigrator.csproj", "database/Krake.DatabaseMigrator/"]
RUN dotnet restore "database/Krake.DatabaseMigrator/Krake.DatabaseMigrator.csproj"
COPY . .
WORKDIR "/src/database/Krake.DatabaseMigrator"
RUN dotnet build "Krake.DatabaseMigrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Krake.DatabaseMigrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Krake.DatabaseMigrator.dll"]
