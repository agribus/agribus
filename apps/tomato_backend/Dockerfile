# Dockerfile.dev
FROM mcr.microsoft.com/dotnet/sdk:9.0

RUN apt-get update && apt-get install -y curl

WORKDIR /app

COPY Agribus.sln .
COPY src/Agribus.Core/Agribus.Core.csproj src/Agribus.Core/Agribus.Core.csproj

RUN dotnet restore

# TODO
