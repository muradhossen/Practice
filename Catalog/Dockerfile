FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src 

COPY ["Catalog/Catalog.csproj", "Catalog/"] 

RUN dotnet restore "Catalog/Catalog.csproj"

COPY Shared/ Shared/
COPY Catalog/ Catalog/


WORKDIR "/src/Catalog"

RUN dotnet build "Catalog.csproj" -c Release -o /app/build

RUN dotnet publish "Catalog.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app

EXPOSE 5002

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Catalog.dll"] 
