package org.example.keycloak.config;

import com.rabbitmq.client.Channel;
import com.rabbitmq.client.Connection;
import com.rabbitmq.client.ConnectionFactory;
import org.springframework.stereotype.Component;

@Component
public class RabbitMQConfig {

    private static final String EXCHANGE_NAME = "user.exchange";

    public Channel createChannel() throws Exception {
        ConnectionFactory factory = new ConnectionFactory();
        factory.setHost("rabbitmq"); // RabbitMQ-containernaam in Docker
        factory.setPort(5672);
        factory.setUsername("guest");
        factory.setPassword("guest");

        Connection connection = factory.newConnection();
        Channel channel = connection.createChannel();
        channel.exchangeDeclare(EXCHANGE_NAME, "fanout", true);
        return channel;
    }

}
