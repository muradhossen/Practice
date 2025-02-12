FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src 

COPY ["Authentication/Authentication.csproj", "Authentication/"] 

RUN dotnet restore "Authentication/Authentication.csproj"

COPY Shared/ Shared/
COPY Authentication/ Authentication/


WORKDIR "/src/Authentication"

RUN dotnet build "Authentication.csproj" -c Release -o /app/build

RUN dotnet publish "Authentication.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app

EXPOSE 5001

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Authentication.dll"] 
