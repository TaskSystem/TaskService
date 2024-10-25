using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService
{
    public class Notification : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public Notification()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" }; // Zorg dat dit overeenkomt met je RabbitMQ-setup
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "task_created_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var taskCreatedEvent = JsonSerializer.Deserialize<TaskCreatedEvent>(message);

                if (taskCreatedEvent != null)
                {
                    SendEmailNotification(taskCreatedEvent);
                }
            };
            _channel.BasicConsume(queue: "task_created_queue", autoAck: true, consumer: consumer);
            Console.WriteLine(" [*] Waiting for task creation events.");
        }

        private void SendEmailNotification(TaskCreatedEvent taskEvent)
        {
            using (var smtpClient = new SmtpClient("smtp.example.com", 587)) // Pas aan naar je SMTP-server
            {
                smtpClient.Credentials = new NetworkCredential("your-email@example.com", "your-password");
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("your-email@example.com"),
                    Subject = "New Task Created!",
                    Body = $"Hello {taskEvent.Email}, a new task '{taskEvent.TaskName}' was created on your account!",
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(taskEvent.Email);

                try
                {
                    smtpClient.Send(mailMessage);
                    Console.WriteLine($" [x] Sent email to {taskEvent.Email}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" [!] Failed to send email: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}

