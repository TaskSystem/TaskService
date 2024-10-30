using TaskService.Model;
using TaskService.Repository;

namespace TaskService.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly List<TaskModel> _tasks = new List<TaskModel>();

        public IEnumerable<TaskModel> GetAllTasks() => _tasks;

        public TaskModel GetTaskById(Guid id) => _tasks.FirstOrDefault(t => t.Id == id);
        public void AddTask(TaskModel task)
        {
            task.Id = Guid.NewGuid();
            _tasks.Add(task);

        }

        public void DeleteTask(Guid id)
        {
            TaskModel task = GetTaskById(id);
            if (task != null)
            {
                _tasks.Remove(task);
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
            }
        }

        public void AddCommentToTask(Guid taskId, Comment comment)
        {
            var task = GetTaskById(taskId);
            if (task != null)
            {
                comment.Id = Guid.NewGuid();
                comment.CreatedAt = DateTime.UtcNow;
                task.Comments.Add(comment);
            }
        }

        public IEnumerable<Comment> GetCommentsByTaskId(Guid taskId)
        {
            var task = GetTaskById(taskId);
            return task?.Comments ?? Enumerable.Empty<Comment>();
        }

    }
}
