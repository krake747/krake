# Krake

Krake Angular Web Application Demo [krake-app](https://krake747.github.io/krake-angular/home)

## Modular Monolith Architecture

Building a modular monolith.

## Docker

### Certificates

In the Krake.Api project execute the following commands.

```bash
cd ./src/Krake.Api/
dotnet dev-certs https --clean
mkdir certificates
cd ./certificates
dotnet dev-certs https -ep cert.pfx -p Test1234!
dotnet dev-certs https --trust
```

### Docker clean up script

```docker
docker compose down
docker build -f .\src\Krake.Api\Dockerfile -t krake.api .
docker image prune -f
docker compose up -d
```

## User Secrets

For using Redis and KeyCloak on local development add a .NET UserSecrets file and paste the below into it.

```json
{
    "ConnectionStrings": {
        "RedisCache": "localhost:6379"
    },
    "Authentication": {
        "Audience": "account",
        "TokenValidationParameters": {
            "ValidIssuers": [
                "http://krake.identity:8080/realms/krake",
                "http://localhost:18080/realms/krake"
            ]
        },
        "MetadataAddress": "http://localhost:18080/realms/krake/.well-known/openid-configuration",
        "RequireHttpsMetadata": false
    },
    "KeyCloak": {
        "HealthUrl": "http://localhost:18080/health/"
    }
}
```