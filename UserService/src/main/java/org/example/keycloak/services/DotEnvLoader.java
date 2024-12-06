package org.example.keycloak.services;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.stream.Stream;

public class DotEnvLoader {

    public static void loadEnv() {
        try (Stream<String> lines = Files.lines(Paths.get("./.env"))) {
            lines.filter(line -> !line.startsWith("#") && line.contains("="))
                    .forEach(line -> {
                        String[] parts = line.split("=", 2);
                        if (parts.length == 2) {
                            System.setProperty(parts[0], parts[1]);
                        }
                    });
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
