package org.example.keycloak.Controller;

import org.example.keycloak.Model.LoginRequest;
import org.example.keycloak.Model.UserDTO;
import org.example.keycloak.services.JwtTokenProvider;
import org.example.keycloak.services.KeyCloakService;
import org.example.keycloak.services.UserCreation;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;


@RestController
@RequestMapping("/user")
public class UserController {

    private  JwtTokenProvider jwtTokenProvider;

    private final UserCreation userCreation;

    @Autowired
    private KeyCloakService keycloakService;

    @Autowired
    public UserController(UserCreation userCreation) {
        this.userCreation = userCreation;
    }

    @PostMapping("/register")
    public ResponseEntity<String> registerUser(@RequestBody UserDTO userDTO) {
        String userId = userCreation.createUser(userDTO.getEmail(), userDTO.getPassword());
        if (userId != null) {
            return ResponseEntity.ok("User created with ID: " + userId);
        } else {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("Error creating user");
        }
    }

    @PostMapping("/login")
    public ResponseEntity<String> loginUser(@RequestBody LoginRequest loginRequest) {
        try {
            // Verifieer de gebruikersgegevens via Keycloak
            String jwtToken = keycloakService.authenticateWithKeycloak(loginRequest);

            // Als er geen token is, is de authenticatie mislukt
            if (jwtToken == null) {
                return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body("Invalid email or password");
            }

            // Retourneer het JWT-token
            return ResponseEntity.ok("Bearer " + jwtToken);

        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("Exception occurred");
        }
    }
}
