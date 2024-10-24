using System.Xml.Linq;
using TaskService.DTO;

namespace TaskService.Model
{
    public class TaskModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedUser { get; set; }  // User assigned to the task
        // public string Priority { get; set; }      // Priority levels (e.g., "Low", "Medium", "High")
        public bool IsCompleted { get; set; }     // Task completion status
                                                  // public List<string> Files { get; set; }   // File URLs or names associated with the task

        public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();

    }
}
