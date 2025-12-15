# Multi-stage Dockerfile for LinguaTech.API (targeting .NET 8.0)
# Place this file at repository root (next to LinguaTech.sln)
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["LinguaTech.API/LinguaTech.API.csproj", "LinguaTech.API/"]
COPY ["LinguaTech.Application/LinguaTech.Application.csproj", "LinguaTech.Application/"]
COPY ["LinguaTech.Infrastructure/LinguaTech.Infrastructure.csproj", "LinguaTech.Infrastructure/"]
COPY ["LinguaTech.Domain/LinguaTech.Domain.csproj", "LinguaTech.Domain/"]

RUN dotnet restore "LinguaTech.API/LinguaTech.API.csproj"

# Copy everything else and publish
COPY . .
WORKDIR /src/LinguaTech.API
RUN dotnet publish "LinguaTech.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Listen on port 80 inside the container
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "LinguaTech.API.dll"]
