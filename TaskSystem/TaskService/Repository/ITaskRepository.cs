using TaskService.DTO;
using TaskService.Model;

namespace TaskService.Repository
{
    public interface ITaskRepository
    {

        IEnumerable<TaskModel> GetAllTasks();
        TaskModel GetTaskById(Guid id);
        void AddTask(TaskModel task);
        void UpdateTask(TaskModel task);
        void DeleteTask(Guid id);

        void AddCommentToTask(Guid taskId, Comment comment);

        IEnumerable<Comment> GetCommentsByTaskId(Guid taskId);

        IEnumerable<Board> GetAllBoards(); // Voeg deze regel toe
        Board GetBoardById(int id);
        Board GetBoardByName(string name);
        void AddBoard(CreatedBoardDTO boardDto);
        void DeleteBoard(int id);
        void UpdateBoard(int id, UpdatedBoardDTO boardDto);

    }
}
