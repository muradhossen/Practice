FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["Gateway/Gateway.csproj", "Gateway/"]

RUN dotnet restore "Gateway/Gateway.csproj"

COPY Gateway/ Gateway/

WORKDIR "/src/Gateway"

RUN dotnet build "Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

COPY "Gateway/Ocelot.json" "/app/Gateway/Ocelot.json"

RUN dotnet publish "Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app

EXPOSE 5000

COPY --from=build /app/publish .

RUN apt-get update && apt-get install -y curl 

ENTRYPOINT ["dotnet", "Gateway.dll"]