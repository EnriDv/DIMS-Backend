# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder

WORKDIR /src

COPY ["DIMS-Backend.csproj", "./"]

RUN dotnet restore "DIMS-Backend.csproj"

COPY . .

RUN dotnet build "DIMS-Backend.csproj" -c Release -o /app/build

# Publish stage
FROM builder AS publish

RUN dotnet publish "DIMS-Backend.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Install dumb-init for proper signal handling
RUN apt-get update && apt-get install -y --no-install-recommends dumb-init && rm -rf /var/lib/apt/lists/*

RUN useradd -m -u 1001 appuser

COPY --from=publish --chown=appuser:appuser /app/publish .

USER appuser

EXPOSE 8000

ENV ASPNETCORE_URLS=http://+:8000 \
    ASPNETCORE_ENVIRONMENT=Production

# Start application using dumb-init
ENTRYPOINT ["dumb-init", "--"]
CMD ["dotnet", "DIMS-Backend.dll"]
