namespace TaskService.Model
{
    public class TaskCreatedEvent
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
    }
}
