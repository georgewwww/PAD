FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY . .

RUN dotnet restore "./WebServer/WebServer.csproj"
RUN dotnet build "./WebServer/WebServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./WebServer/WebServer.csproj" -c Release -o /app/WebServer

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/WebServer .
ENTRYPOINT ["dotnet", "WebServer.dll"]