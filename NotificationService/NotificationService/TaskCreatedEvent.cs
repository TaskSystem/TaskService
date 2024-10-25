namespace NotificationService
{
    public class TaskCreatedEvent
    {
        public string Email { get; set; }
        public string TaskName { get; set; }
    }
}
