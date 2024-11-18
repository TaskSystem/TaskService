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
                comment.Id = Guid.NewGuid();  // Genereer een unieke ID voor de comment
                comment.TaskId = taskId;
                comment.CreatedAt = DateTime.UtcNow;
                task.Comments.Add(comment);
            }
        }

        public IEnumerable<Comment> GetCommentsByTaskId(Guid taskId)
        {
            var task = GetTaskById(taskId);
            return task?.Comments ?? Enumerable.Empty<Comment>();
        }


        public IEnumerable<Board> GetAllBoards() => _boards;

        public Board GetBoardById(int id) => _boards.FirstOrDefault(b => b.Id == id);

        public Board GetBoardByName(string name) => _boards.FirstOrDefault(b => b.Title.Equals(name, StringComparison.OrdinalIgnoreCase));

        public void AddBoard(CreatedBoardDTO boardDto)
        {
            var newBoard = new Board
            {
                Id = _boardIdCounter++,
                Title = boardDto.Title,
                Description = boardDto.Description,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            _boards.Add(newBoard);
        }

        public void DeleteBoard(int id)
        {
            var board = GetBoardById(id);
            if (board != null)
            {
                _boards.Remove(board);
            }
        }

        public void UpdateBoard(int id, UpdatedBoardDTO boardDto)
        {
            var board = GetBoardById(id);
            if (board != null)
            {
                board.Title = boardDto.Title;
                board.Description = boardDto.Description;
                board.UpdatedDate = DateTime.UtcNow;
            }
        }

    }
}
