#!/bin/bash

# Start the containers defined in docker-compose.yml.
docker compose up -d

# Run the integration tests.
dotnet test -c Release --no-restore --verbosity normal

# Stop and remove the containers.
docker-compose down