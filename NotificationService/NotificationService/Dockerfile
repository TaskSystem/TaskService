# Gebruik een officiële .NET SDK-image als buildomgeving
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Kopieer csproj en restore de dependencies
COPY *.csproj ./
RUN dotnet restore

# Kopieer de rest van de projectbestanden en bouw de toepassing in Release mode
COPY . ./
RUN dotnet publish -c Release -o out

# Gebruik een kleinere runtime-image voor de app zelf
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Kopieer de output van de buildomgeving naar de runtimeomgeving
COPY --from=build-env /app/out .

# Expose de poort waarop de service luistert
EXPOSE 80

# Start de NotificationService
ENTRYPOINT ["dotnet", "NotificationService.dll"]
