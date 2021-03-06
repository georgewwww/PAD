FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY . .

RUN dotnet restore "./MessageBus/MessageBus.csproj"
RUN dotnet build "./MessageBus/MessageBus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./MessageBus/MessageBus.csproj" -c Release -o /app/MessageBus

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/MessageBus .
ENTRYPOINT ["dotnet", "MessageBus.dll"]