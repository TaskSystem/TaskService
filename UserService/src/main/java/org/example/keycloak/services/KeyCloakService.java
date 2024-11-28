package org.example.keycloak.services;

import com.fasterxml.jackson.databind.ObjectMapper;
import org.apache.http.HttpResponse;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.HttpClients;
import org.example.keycloak.Model.LoginRequest;
import org.keycloak.OAuth2Constants;
import org.keycloak.common.util.KeycloakUriBuilder;
import org.keycloak.representations.AccessTokenResponse;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

@Service
public class KeyCloakService {

    @Value("${KEYCLOAK_AUTH_SERVER_URL}")
    private String authServerUrl;

    @Value("${KEYCLOAK_REALM}")
    private String realm;

    @Value("${KEYCLOAK_CLIENT_ID}")
    private String clientId;



    // Verifieer de gebruikersnaam en het wachtwoord
    public String authenticateWithKeycloak(LoginRequest loginRequest) {
        // Bouw de URL voor het token endpoint
        String tokenEndpoint = KeycloakUriBuilder.fromUri(authServerUrl)
                .path("/realms/" + realm + "/protocol/openid-connect/token")
                .build()
                .toString();

        // Maak een POST-verzoek naar Keycloak om de gebruiker te verifiëren
        try {
            // Stel de body in voor de password grant
            String body = "grant_type=" + OAuth2Constants.PASSWORD +
                    "&client_id=" + clientId +
                    "&username=" + loginRequest.getEmail() +
                    "&password=" + loginRequest.getPassword();

            // Verstuur de POST-verzoek
            try (CloseableHttpClient client = HttpClients.createDefault()) {
                HttpPost post = new HttpPost(tokenEndpoint);
                post.setEntity(new StringEntity(body));
                post.setHeader("Content-Type", "application/x-www-form-urlencoded");

                HttpResponse response = client.execute(post);

                // Controleer de statuscode van de response
                if (response.getStatusLine().getStatusCode() == 200) {
                    ObjectMapper objectMapper = new ObjectMapper();
                    AccessTokenResponse tokenResponse = objectMapper.readValue(response.getEntity().getContent(), AccessTokenResponse.class);

                    // Als het verzoek succesvol is, retourneer dan het JWT-token
                    return tokenResponse.getToken();  // Het token is nu geldig voor deze gebruiker
                } else {
                    return null;  // Fout bij inloggen
                }
            }
        } catch (Exception e) {
            e.printStackTrace();
            return null;  // Fout bij de aanvraag
        }
    }
}
