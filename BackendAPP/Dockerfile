# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the whole BackendAPP folder (because project is nested)
COPY . .

# Go into the project subfolder (where csproj actually is)
WORKDIR /src/BackendAPP

# Restore
RUN dotnet restore

# Publish
RUN dotnet publish -c Release -o /app/out


# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/out ./

# PORT comes from Render
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

EXPOSE 8080

ENTRYPOINT ["dotnet", "BackendAPP.dll"]
