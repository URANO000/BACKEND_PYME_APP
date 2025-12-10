# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore
COPY BackendAPP.csproj ./
RUN dotnet restore

# Copy everything and publish
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/out ./

# Render provides PORT environment variable
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

EXPOSE 8080

ENTRYPOINT ["dotnet", "BackendAPP.dll"]
