package org.example.keycloak.services;

import org.springframework.beans.factory.annotation.Value;

import java.util.Date;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.SignatureAlgorithm;
import org.springframework.stereotype.Component;

@Component
public class JwtTokenProvider {

    @Value("${jwt.secret}")
    private String jwtSecret;

    @Value("${jwt.expiration}")
    private long jwtExpirationMs;

    public String generateTokenFromKeycloak(String keycloakJwtToken) {
        // Gebruik de Keycloak tokeninformatie om je eigen JWT te genereren voor backend-toegang
        return Jwts.builder()
                .setSubject("keycloak-user")
                .setIssuedAt(new Date())
                .setExpiration(new Date(System.currentTimeMillis() + jwtExpirationMs))
                .signWith(SignatureAlgorithm.HS512, jwtSecret)
                .compact();
    }
}
