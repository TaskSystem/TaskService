FROM eclipse-temurin:17-jdk-alpine

# Stel de werkmap in
WORKDIR /app

# Kopieer de applicatie JAR naar de container
COPY target/KeyCloak-0.0.1-SNAPSHOT.jar app.jar

# Kopieer het .env-bestand naar de container
COPY .env .env

# Start de applicatie
ENTRYPOINT ["java", "-jar", "app.jar"]
