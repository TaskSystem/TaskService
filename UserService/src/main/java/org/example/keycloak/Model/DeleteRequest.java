package org.example.keycloak.Model;

import java.util.UUID;

public class DeleteRequest {
    private UUID userId;

    // Getters en setters
    public UUID getUserId() {
        return userId;
    }

    public void setUserId(UUID userId) {
        this.userId = userId;
    }
}

