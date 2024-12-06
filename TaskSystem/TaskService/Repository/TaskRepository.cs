using TaskService.DTO;
using TaskService.Model;
using TaskService.Repository;

namespace TaskService.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly List<TaskModel> _tasks = new List<TaskModel>();
        private readonly List<Board> _boards = new List<Board>();
        private int _boardIdCounter = 1;

        // --- Taken ---

        public IEnumerable<TaskModel> GetAllTasks() => _tasks;

        public TaskModel GetTaskById(Guid id) => _tasks.FirstOrDefault(t => t.Id == id);

        public IEnumerable<TaskModel> GetTasksByUserId(string userId) =>
            _tasks.Where(t => t.AssignedUser == userId);

        public void AddTask(TaskModel task)
        {
            task.Id = Guid.NewGuid();
            _tasks.Add(task);
            Console.WriteLine($"Taak toegevoegd: {task.Title} (ID: {task.Id}, UserId: {task.AssignedUser})");
        }

        public void DeleteTask(Guid id)
        {
            TaskModel task = GetTaskById(id);
            if (task != null)
            {
                _tasks.Remove(task);
                Console.WriteLine($"Taak verwijderd: {task.Title} (ID: {task.Id})");
            }
        }

        public void DeleteTasksByUserId(Guid userId)
        {
            var tasksToDelete = _tasks.Where(task => task.Comments.Any(comment => comment.UserId == userId)).ToList();
            foreach (var task in tasksToDelete)
            {
                _tasks.Remove(task);
                Console.WriteLine($"Taak verwijderd: {task.Title} (ID: {task.Id}) gekoppeld aan UserId: {userId}");
            }
        }

        public void UpdateTask(TaskModel updatedTask)
        {
            TaskModel existingTask = GetTaskById(updatedTask.Id);
            if (existingTask != null)
            {
                existingTask.Title = updatedTask.Title;
                existingTask.Description = updatedTask.Description;
                existingTask.AssignedUser = updatedTask.AssignedUser;
                existingTask.IsCompleted = updatedTask.IsCompleted;

                Console.WriteLine($"Taak bijgewerkt: {existingTask.Title} (ID: {existingTask.Id})");
            }
        }

        public void AddCommentToTask(Guid taskId, Comment comment)
        {
            var task = GetTaskById(taskId);
            if (task != null)
            {
                comment.Id = Guid.NewGuid(); // Genereer een unieke ID voor de comment
                comment.TaskId = taskId;
                comment.CreatedAt = DateTime.UtcNow;
                task.Comments.Add(comment);

                Console.WriteLine($"Comment toegevoegd aan taak: {task.Title} (Task ID: {task.Id})");
            }
        }

        public IEnumerable<Comment> GetCommentsByTaskId(Guid taskId)
        {
            var task = GetTaskById(taskId);
            return task?.Comments ?? Enumerable.Empty<Comment>();
        }

        public IEnumerable<TaskModel> GetTasksByUserIdInComments(Guid userId)
        {
            return _tasks.Where(t => t.Comments.Any(c => c.UserId == userId));
        }
    }
}
