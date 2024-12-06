package org.example.keycloak.Controller;

import com.rabbitmq.client.Channel;
import org.example.keycloak.Model.DeleteRequest;
import org.example.keycloak.Model.LoginRequest;
import org.example.keycloak.Model.UserDTO;
import org.example.keycloak.config.RabbitMQConfig;
import org.example.keycloak.services.JwtTokenProvider;
import org.example.keycloak.services.KeyCloakService;
import org.example.keycloak.services.UserCreation;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;


@RestController
@RequestMapping("/user")
public class UserController {

    private  JwtTokenProvider jwtTokenProvider;

    private final UserCreation userCreation;

    @Autowired
    private KeyCloakService keycloakService;

    private final RabbitMQConfig rabbitMQConfig;

    //private UserInfo userInfoo;

    @Autowired
    public UserController(UserCreation userCreation, RabbitMQConfig rabbitMQConfig) {
        this.userCreation = userCreation;
        this.rabbitMQConfig = rabbitMQConfig;
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

    @DeleteMapping("/delete")
    public ResponseEntity<String> deleteUser(@RequestBody DeleteRequest deleteUserRequest) {
        try {
            UUID userId = deleteUserRequest.getUserId();

            System.out.println("Received DELETE request for userId: " + userId);
            System.out.println("Authorization Header: " + deleteUserRequest);

            boolean success = keycloakService.deleteUser(userId);
            if (success) {
                // RabbitMQ-logica
                try {
                    Channel channel = rabbitMQConfig.createChannel();
                    String message = "{\"userId\":\"" + userId + "\"}";
                    channel.basicPublish("user.exchange", "", null, message.getBytes());
                    System.out.println("Verwijderbericht verzonden naar RabbitMQ: " + message);
                } catch (Exception e) {
                    System.err.println("RabbitMQ bericht verzenden mislukt: " + e.getMessage());
                    return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("User deleted but RabbitMQ failed.");
                }

                return ResponseEntity.ok("User deleted successfully");
            } else {
                return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("Failed to delete user");
            }
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("Error occurred while deleting user");
        }
    }

   /* @GetMapping("userinfo")
    public ResponseEntity<UserDTO> getUserInfo() {
        try {
            String userId = userInfoo.getAuthenticatedUserId();

            UserDTO userInfo = userInfoo.getUserInfo(userId);

            if (userInfo != null) {
                return ResponseEntity.ok(userInfo);
            } else {
                return ResponseEntity.status(HttpStatus.NOT_FOUND).body(null);
            }

        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(null);
        }
    }*/
}
