using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using TaskService.Model;
using System.Collections.Concurrent;
using TaskService.Repository;

namespace TaskService.PublishRMQ
{
    public class Consumer
    {
        private const string ExchangeName = "user.exchange";
        private const string QueueName = "user_deletion_queue";
        private readonly ITaskRepository _taskRepository;
        private readonly IConnection _connection;
        private IModel _channel;

        public Consumer(ITaskRepository taskRepository, IConnection connection)
        {
            _taskRepository = taskRepository;
            _connection = connection;
        }

        public void StartListening()
        {
            _channel = _connection.CreateModel();

            // Declareer de exchange en een persistente queue
            _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout, durable: true);
            _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: "");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Ontvangen bericht: {message}");

                var userDeletionEvent = JsonConvert.DeserializeObject<UserDeletionEvent>(message);

                if (userDeletionEvent != null)
                {
                    Console.WriteLine($"Start met verwijderen van taken voor gebruiker: {userDeletionEvent.UserId}");
                    DeleteUserData(userDeletionEvent.UserId);
                    Console.WriteLine($"Taken succesvol verwijderd voor gebruiker: {userDeletionEvent.UserId}");
                }
                else
                {
                    Console.WriteLine("Ontvangen bericht kon niet worden gedeserialiseerd.");
                }

                // Handmatige bevestiging
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

            Console.WriteLine("Wachten op berichten...");
            Console.ReadLine(); // Houd de consumer actief
        }

        private async Task DeleteUserData(Guid userId)
        {
            Console.WriteLine($"[Consumer] Verwijderen van taken waar userId {userId} in comments voorkomt");

            // Haal de taken asynchroon op
            var tasksBefore = await _taskRepository.GetTasksByUserIdInComments(userId);
            Console.WriteLine($"[Consumer] Taken voor verwijdering: {tasksBefore.Count()}");

            // Verwijder de taken asynchroon
            await _taskRepository.DeleteTasksByUserId(userId);

            // Haal de taken na verwijdering asynchroon op
            var tasksAfter = await _taskRepository.GetTasksByUserIdInComments(userId);
            Console.WriteLine($"[Consumer] Taken na verwijdering: {tasksAfter.Count()}");
        }

    }
}
