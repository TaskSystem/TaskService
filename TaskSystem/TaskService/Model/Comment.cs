namespace TaskService.Model
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }  // Dit kan optioneel zijn als het altijd bij TaskModel wordt geladen
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
