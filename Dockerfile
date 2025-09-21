# Use the official .NET 8.0 runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 8.0 SDK as the build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["BHCARE.csproj", "."]
RUN dotnet restore "BHCARE.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "BHCARE.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "BHCARE.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "BHCARE.dll"]
