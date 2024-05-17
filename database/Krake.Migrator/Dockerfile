FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["database/Krake.Migrator/Krake.Migrator.csproj", "database/Krake.Migrator/"]
RUN dotnet restore "database/Krake.Migrator/Krake.Migrator.csproj"
COPY . .
WORKDIR "/src/database/Krake.Migrator"
RUN dotnet build "Krake.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Krake.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Krake.Migrator.dll"]
