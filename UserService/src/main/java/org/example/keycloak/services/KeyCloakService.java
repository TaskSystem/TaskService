package org.example.keycloak.services;

import com.fasterxml.jackson.databind.ObjectMapper;
import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.methods.HttpDelete;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.HttpClients;
import org.example.keycloak.Model.LoginRequest;
import org.example.keycloak.Model.UserDTO;
import org.keycloak.KeycloakPrincipal;
import org.keycloak.OAuth2Constants;
import org.keycloak.common.util.KeycloakUriBuilder;
import org.keycloak.representations.AccessTokenResponse;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.*;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;

import java.util.Map;
import java.util.UUID;

@Service
public class KeyCloakService {

    @Value("${KEYCLOAK_AUTH_SERVER_URL}")
    private String authServerUrl;

    @Value("${KEYCLOAK_REALM}")
    private String realm;

    @Value("${KEYCLOAK_CLIENT_ID}")
    private String clientId;

    @Value("${KEYCLOAK_USER_ADMIN}")
    private String adminUsername;

    @Value("${KEYCLOAK_USER_PASSWORD}")
    private String adminPassword;



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


    public boolean deleteUser(UUID userId) {
        try {
            // Verkrijg een admin access token via username-password flow
            String tokenEndpoint = KeycloakUriBuilder.fromUri(authServerUrl)
                    .path("/realms/" + realm + "/protocol/openid-connect/token")
                    .build()
                    .toString();

            String body = "grant_type=" + OAuth2Constants.PASSWORD +
                    "&client_id=" + clientId +
                    "&username=" + adminUsername +
                    "&password=" + adminPassword;

            String adminToken;
            try (CloseableHttpClient client = HttpClients.createDefault()) {
                HttpPost post = new HttpPost(tokenEndpoint);
                post.setEntity(new StringEntity(body));
                post.setHeader("Content-Type", "application/x-www-form-urlencoded");

                HttpResponse response = client.execute(post);
                if (response.getStatusLine().getStatusCode() == 200) {
                    ObjectMapper objectMapper = new ObjectMapper();
                    Map<String, Object> tokenResponse = objectMapper.readValue(response.getEntity().getContent(), Map.class);
                    adminToken = tokenResponse.get("access_token").toString();
                } else {
                    throw new RuntimeException("Failed to retrieve admin token");
                }
            }

            String userEndpoint = authServerUrl + "/admin/realms/" + realm + "/users/" + userId;
            System.out.println("User Endpoint: " + userEndpoint);

            // Maak de DELETE-aanroep om de gebruiker te verwijderen
            try (CloseableHttpClient client = HttpClients.createDefault()) {
                HttpDelete deleteRequest = new HttpDelete(userEndpoint);
                deleteRequest.setHeader("Authorization", "Bearer " + adminToken);

                HttpResponse response = client.execute(deleteRequest);
                return response.getStatusLine().getStatusCode() == 204; // 204 betekent dat de gebruiker is verwijderd
            }
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }
    }


}
