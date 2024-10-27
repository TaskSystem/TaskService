using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace NotificationService
{
    public class NotificationService : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly EmailService _emailService;

        public NotificationService(EmailService emailService)
        {
            _emailService = emailService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Start listening for incoming messages
            StartListening(stoppingToken);
            return Task.CompletedTask;
        }

        private void StartListening(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" }; // Use the service name

            int retries = 10;
            while (retries > 0)
            {
                try
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    _channel.QueueDeclare(queue: "task_created_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var consumer = new EventingBasicConsumer(_channel);
                    consumer.Received += async (model, ea) =>
                    {
                        try
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            var taskCreatedEvent = JsonSerializer.Deserialize<TaskCreatedEvent>(message);

                            if (taskCreatedEvent != null)
                            {
                                await _emailService.SendEmailAsync(taskCreatedEvent.Email, "New Task Created!",
                                    $"Hello, a new task '{taskCreatedEvent.TaskName}' was created!",
                                    $"<strong>Hello, a new task '{taskCreatedEvent.TaskName}' was created!</strong>");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing message: {ex.Message}");
                        }
                    };

                    _channel.BasicConsume(queue: "task_created_queue", autoAck: true, consumer: consumer);
                    Console.WriteLine(" [*] Waiting for task creation events.");
                    break;
                }
                catch (BrokerUnreachableException)
                {
                    Console.WriteLine("Retrying to connect to RabbitMQ...");
                    retries--;
                    Thread.Sleep(2000);
                }
            }

            if (_connection == null)
            {
                Console.WriteLine("Failed to connect to RabbitMQ after retries.");
                return;
            }
        }

        public override void Dispose()
        {
            // Clean up the resources
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
