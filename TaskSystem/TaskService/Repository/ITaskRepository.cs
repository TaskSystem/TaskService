using TaskService.DTO;
using TaskService.Model;

namespace TaskService.Repository
{
    public interface ITaskRepository
    {

        // Verkrijg alle taken
        Task<IEnumerable<TaskModel>> GetAllTasks();

        // Verkrijg taak op basis van ID
        Task<TaskModel> GetTaskById(Guid id);

        // Voeg een taak toe
        Task AddTask(TaskModel task);

        // Update een taak
        Task UpdateTask(TaskModel task);

        // Verwijder taak op basis van ID
        Task DeleteTask(Guid id);

        // Verwijder taken gekoppeld aan een gebruiker
        Task DeleteTasksByUserId(Guid userId);

        // Voeg comment toe aan taak
        Task AddCommentToTask(Guid taskId, Comment comment);

        // Verkrijg comments van een taak op basis van taak ID
        Task<IEnumerable<Comment>> GetCommentsByTaskId(Guid taskId);

        // Verkrijg taken gebaseerd op gebruiker ID in comments
        Task<IEnumerable<TaskModel>> GetTasksByUserIdInComments(Guid userId);


    }
}
