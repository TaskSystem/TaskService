package org.example.keycloak;

import org.example.keycloak.services.DotEnvLoader;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

@SpringBootApplication
public class KeyCloakApplication {

	public static void main(String[] args) {

		DotEnvLoader.loadEnv();
		SpringApplication.run(KeyCloakApplication.class, args);
	}

}
