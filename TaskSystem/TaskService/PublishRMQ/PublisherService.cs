using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class PublisherService : IDisposable
{
    //private readonly IConnection _connection;
    private readonly IModel _channel;

    public PublisherService(IConnection connection)
    {
        //var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        //_connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "task_created_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void Publish<T>(T eventMessage)
    {
        var message = JsonSerializer.Serialize(eventMessage);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "", routingKey: "task_created_queue", basicProperties: null, body: body);
        Console.WriteLine($"Bericht gepubliceerd: {message}");
    }

    public void Dispose()
    {
        _channel.Close();
    }
}

