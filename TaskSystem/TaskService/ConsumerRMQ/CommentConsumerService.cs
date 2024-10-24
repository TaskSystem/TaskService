using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Xml.Linq;
using TaskService.Repository;
using System.Text;
using System.Text.Json;
using TaskService.DTO;

namespace TaskService.ConsumerRMQ
{
    public class CommentConsumerService
    {
        private readonly ConnectionFactory _factory;
        private readonly ITaskRepository _taskRepository;

        public CommentConsumerService(ITaskRepository taskRepository)
        {
            _factory = new ConnectionFactory() { HostName = "rabbitmq" };
            _taskRepository = taskRepository;
        }

        public void StartConsuming()
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_comment_queue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    // Deserializeer het bericht
                    var commentData = JsonSerializer.Deserialize<CommentDTO>(message);

                    // Verwerk de comment en voeg deze toe aan de juiste task
                    ProcessComment(commentData);
                };

                channel.BasicConsume(queue: "task_comment_queue",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Consuming from RabbitMQ");
            }
        }

        private void ProcessComment(CommentDTO commentDto)
        {
            // Haal de juiste taak op op basis van TaskId
            var task = _taskRepository.GetTaskById(commentDto.TaskId);
            if (task != null)
            {
                // Voeg de comment toe aan de task
                task.Comments.Add(new CommentDTO
                {
                    Content = commentDto.Content,
                });

                // Sla de wijzigingen op
                _taskRepository.UpdateTask(task);
            }
        }

        public void HandleCommentForTask(Guid taskId, CommentDTO comment)
        {
            // Veronderstel dat je de task ophaalt vanuit een repository
            var task = _taskRepository.GetTaskById(taskId);

            if (task != null)
            {
                // Controleer of task wel een Comments-collectie heeft
                if (task.Comments == null)
                {
                    task.Comments = new List<CommentDTO>();
                }

                // Voeg de comment toe aan de lijst
                task.Comments.Add(comment);

                // Werk de taak bij in de repository
                _taskRepository.UpdateTask(task);
            }
        }

    }

}
