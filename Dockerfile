# Base runtime image (only runs the app)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build image (SDK includes compiler, restore, build tools)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first (for better Docker layer caching)
COPY ["API/API.csproj", "API/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Shared/Shared.csproj", "Shared/"]

# Restore dependencies
RUN dotnet restore "API/API.csproj"

# Copy everything else
COPY . .

# Set working directory to API project
WORKDIR /src/API

# Build and publish into /app/publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Start application
ENTRYPOINT ["dotnet", "API.dll"]