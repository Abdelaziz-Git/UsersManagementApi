# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies (cached layer)
COPY UsersManagementApi.csproj .
RUN dotnet restore UsersManagementApi.csproj

# Copy application code and build
COPY . .
RUN dotnet build UsersManagementApi.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish UsersManagementApi.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

# Copy published application from publish stage
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD dotnet --version || exit 1

ENTRYPOINT ["dotnet", "UsersManagementApi.dll"]
