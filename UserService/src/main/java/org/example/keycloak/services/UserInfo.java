/*package org.example.keycloak.services;

import org.springframework.http.HttpEntity;
import org.example.keycloak.Model.UserDTO;
import org.keycloak.KeycloakPrincipal;
import org.springframework.http.*;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;

import java.util.Map;

@Service
public class UserInfo {

    public String getAdminAccessToken(String clientId) throws Exception {
        String tokenUrl = "http://keyclaok:8080/auth/realms/UserService/protocol/openid-connect/token";

        RestTemplate restTemplate = new RestTemplate();

        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_FORM_URLENCODED);

        String requestBody = "client_id=" + clientId +
                "&grant_type=client_credentials";

        HttpEntity<String> entity = new HttpEntity<>(requestBody, headers);

        ResponseEntity<Map> response = restTemplate.exchange(
                tokenUrl,
                HttpMethod.POST,
                entity,
                Map.class
        );

        if (response.getStatusCode() == HttpStatus.OK) {
            Map<String, Object> responseBody = response.getBody();
            return (String) responseBody.get("access_token");
        }

        throw new Exception("Failed to retrieve access token from Keycloak");
    }


    public String getAuthenticatedUserId() {
        Authentication authentication = SecurityContextHolder.getContext().getAuthentication();
        if (authentication != null && authentication.getPrincipal() instanceof KeycloakPrincipal) {
            KeycloakPrincipal<?> keycloakPrincipal = (KeycloakPrincipal<?>) authentication.getPrincipal();
            return keycloakPrincipal.getKeycloakSecurityContext().getToken().getSubject();
        }
        return null;
    }

    public UserDTO getUserInfo(String userId) throws Exception {
        String serverUrl = "http://keycloak:8080/auth/admin/realms/UserService";
        String clientId = "user-client-api";

        String adminToken = getAdminAccessToken(clientId);

        RestTemplate restTemplate = new RestTemplate();
        HttpHeaders headers = new HttpHeaders();
        headers.setBearerAuth(adminToken);

        HttpEntity<String> entity = new HttpEntity<>(headers);
        ResponseEntity<Map> response = restTemplate.exchange(
                serverUrl + "/users/" + userId,
                HttpMethod.GET,
                entity,
                Map.class
        );

        if (response.getStatusCode() == HttpStatus.OK) {
            Map<String, Object> userMap = response.getBody();
            UserDTO userDTO = new UserDTO();
            userDTO.setEmail((String) userMap.get("email"));
            userDTO.setPassword("******");

            return userDTO;
        }

        return null;


    }
}*/
