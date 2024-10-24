# Gebruik de officiële .NET 8.0 SDK image om de applicatie te bouwen
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Kopieer de oplossing en herstel afhankelijkheden
COPY TaskService/*.csproj ./TaskService/
RUN dotnet restore ./TaskService/TaskService.csproj

# Kopieer de rest van de applicatie
COPY . .

# Bouw de applicatie
RUN dotnet publish ./TaskService/TaskService.csproj -c Release -o out

# Maak de runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .  

# Stel de startcommando in
ENTRYPOINT ["dotnet", "TaskService.dll"]
