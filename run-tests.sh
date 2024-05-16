#!/bin/bash

# Start the containers defined in docker-compose.yml.
docker compose up -d

# Run the integration tests.
dotnet test

# Stop and remove the containers.
docker-compose down