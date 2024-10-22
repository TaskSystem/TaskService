# Gebruik de officiële .NET 8.0 SDK image om de applicatie te bouwen
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Kopieer de oplossing en herstel afhankelijkheden
COPY BoardService/BoardService.csproj ./BoardService/
RUN dotnet restore

# Kopieer de rest van de applicatie
COPY . ./
RUN dotnet publish BoardService/BoardService.csproj -c Release -o out

# Maak de runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out ./

# Stel de startcommando in
ENTRYPOINT ["dotnet", "BoardService.dll"]

