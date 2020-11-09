FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY . .

RUN dotnet restore "./SmartProxy/SmartProxy.csproj"
RUN dotnet build "./SmartProxy/SmartProxy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./SmartProxy/SmartProxy.csproj" -c Release -o /app/SmartProxy

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/SmartProxy .
ENTRYPOINT ["dotnet", "SmartProxy.dll"]