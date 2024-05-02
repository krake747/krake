# Krake

Krake Application

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

For using Redis on local development add a .NET UserSecrets file and paste the below.

```json
{
    "ConnectionStrings": {
        "RedisCache": "localhost:6379"
    }
}
```