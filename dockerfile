# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder

WORKDIR /src

# Copy project files
COPY ["DIMS-Backend.csproj", "./"]

# Restore dependencies
RUN dotnet restore "DIMS-Backend.csproj"

# Copy all source code
COPY . .

# Build the application
RUN dotnet build "DIMS-Backend.csproj" -c Release -o /app/build

# Publish stage
FROM builder AS publish

RUN dotnet publish "DIMS-Backend.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Install dumb-init for proper signal handling
RUN apt-get update && apt-get install -y --no-install-recommends dumb-init && rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN useradd -m -u 1001 appuser

# Copy published application from publish stage
COPY --from=publish --chown=appuser:appuser /app/publish .

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8000

# Environment variables (can be overridden at runtime)
ENV ASPNETCORE_URLS=http://+:8000 \
    ASPNETCORE_ENVIRONMENT=Production

# Health check (optional, uncomment if needed)
# HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
#   CMD curl -f http://localhost:8000/health || exit 1

# Start application using dumb-init
ENTRYPOINT ["dumb-init", "--"]
CMD ["dotnet", "DIMS-Backend.dll"]
