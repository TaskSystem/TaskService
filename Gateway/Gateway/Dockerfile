# Gebruik een officiële .NET runtime als basis
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Gebruik de .NET SDK om de applicatie te bouwen
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopieer de projectbestanden
COPY ["Gateway.csproj", "./"]

# Herstel de afhankelijkheden
RUN dotnet restore "Gateway.csproj"

# Kopieer de rest van de bestanden en bouw de applicatie
COPY . .
RUN dotnet publish "Gateway.csproj" -c Release -o /app/publish

# Maak een runtime-image met de gepubliceerde bestanden
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Gateway.dll"]

