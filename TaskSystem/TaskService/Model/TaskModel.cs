using System.Xml.Linq;

namespace TaskService.Model
{
    public class TaskModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string Email { get; set; }
        public string AssignedUser { get; set; }  // User assigned to the task
        // public string Priority { get; set; }      // Priority levels (e.g., "Low", "Medium", "High")
        public bool IsCompleted { get; set; }
        
        public DateTime CreatedAt { get; set; }// Task completion status
                                                  // public List<string> Files { get; set; }   // File URLs or names associated with the task


    }
}
