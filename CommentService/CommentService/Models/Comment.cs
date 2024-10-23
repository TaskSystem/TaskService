namespace CommentService.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }  // ID van de taak waarop deze comment betrekking heeft
        public Guid UserId { get; set; }   // ID van de gebruiker die de comment heeft geplaatst
        public string Content { get; set; } // Inhoud van de comment
        public DateTime CreatedAt { get; set; } // Tijdstip van creatie
    }
}
