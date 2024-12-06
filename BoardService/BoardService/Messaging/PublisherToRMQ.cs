using System.Text.Json;
using System.Text;
using RabbitMQ.Client;

namespace BoardService.Messaging
{
    public class PublisherToRMQ : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public PublisherToRMQ()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "boardchannel", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void Publish<T>(T eventMessage)
        {
            var message = JsonSerializer.Serialize(eventMessage);
            var body = Encoding.UTF8.GetBytes(message);

            Console.WriteLine($"Publishing message to 'boardchannel': {message}");  // Voeg logging toe

            try
            {
                _channel.BasicPublish(exchange: "", routingKey: "boardchannel", basicProperties: null, body: body);
                Console.WriteLine("Message published successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing message: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
