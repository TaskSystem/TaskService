# Stap 1: Gebruik de officiële .NET SDK als basis
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Stel de werkdirectory in
WORKDIR /app

# Kopieer het projectbestand en herstel afhankelijkheden
COPY ./CommentService/*.csproj ./CommentService/
RUN dotnet restore ./CommentService/CommentService.csproj

# Stap 2: Kopieer de rest van de applicatie en bouw deze
COPY ./CommentService/. ./CommentService/
RUN dotnet publish ./CommentService/CommentService.csproj -c Release -o out

# Stap 3: Maak een runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0

# Stel de werkdirectory in
WORKDIR /app

# Kopieer de output van de build
COPY --from=build /app/CommentService/out ./

# Expose poort 8080 (of een andere poort als dat nodig is)
EXPOSE 8080

# Definieer het commando om de applicatie te starten
ENTRYPOINT ["dotnet", "CommentService.dll"]
