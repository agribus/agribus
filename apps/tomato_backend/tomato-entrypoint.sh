#!/bin/sh

# Run migrations
echo "Running migrations..."
dotnet ef database update --project ./src/Agribus.Postgres --startup-project ./src/Agribus.Postgres

# Start the application based on the DEBUG_MODE environment variable
cd ./src/Agribus.Api

echo "Starting application..."
if [ "$DEBUG_MODE" = "true" ]; then
  echo "Running in debug mode"
  exec dotnet run --no-launch-profile
else
  echo "Running in watch mode"
  exec dotnet watch --no-launch-profile run
fi
