FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Krake.Api/Krake.Api.csproj", "src/Krake.Api/"]
RUN dotnet restore "src/Krake.Api/Krake.Api.csproj"
COPY . .
WORKDIR "/src/src/Krake.Api"
RUN dotnet build "Krake.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Krake.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
COPY ["src/Krake.Api/certificates/cert.pfx", "/certificates/cert.pfx"]
ENTRYPOINT ["dotnet", "Krake.Api.dll", "--launch-profile https"]
