using MongoDB.Driver;
using TaskService.DTO;
using TaskService.Model;
using TaskService.MongoDB;
using TaskService.Repository;

namespace TaskService.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly List<TaskModel> _tasks = new List<TaskModel>();
        private readonly List<Board> _boards = new List<Board>();
        private int _boardIdCounter = 1;
        private MongoDBService _mongoDBService;

        public TaskRepository(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // --- Taken ---

        public async Task<IEnumerable<TaskModel>> GetAllTasks()
        {
            var tasksCollection = _mongoDBService.GetTasksCollection();
            var tasks = await tasksCollection.Find(task => true).ToListAsync();
            return tasks;
        }

        // Verkrijg een taak op basis van ID
        public async Task<TaskModel> GetTaskById(Guid id)
        {
            var tasksCollection = _mongoDBService.GetTasksCollection();
            var task = await tasksCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
            return task; // Het resultaat is al een TaskModel, dus we hoeven niets te mappen
        }

        // Verkrijg taken toegewezen aan een gebruiker
        public async Task<IEnumerable<TaskModel>> GetTasksByUserId(string userId)
        {
            var tasksCollection = _mongoDBService.GetTasksCollection();
            var tasks = await tasksCollection.Find(t => t.AssignedUser == userId).ToListAsync();
            return tasks;
        }

        // Voeg een taak toe
        public async Task AddTask(TaskModel task)
        {
            try
            {
                var tasksCollection = _mongoDBService.GetTasksCollection();
                task.Id = Guid.NewGuid();  // Zorg ervoor dat we een nieuwe ID genereren voor de taak
                task.CreatedAt = DateTime.UtcNow;  // Voeg de creation date toe

                // Log de taak die toegevoegd gaat worden
                Console.WriteLine($"Probeer taak toe te voegen: {task.Title} (ID: {task.Id})");

                // Voeg de taak toe aan de MongoDB-collectie
                await tasksCollection.InsertOneAsync(task);

                // Log succes
                Console.WriteLine($"Taak succesvol toegevoegd: {task.Title} (ID: {task.Id})");
            }
            catch (Exception ex)
            {
                // Log fouten bij het toevoegen van de taak
                Console.WriteLine("Fout bij het toevoegen van de taak: " + ex.Message);
            }
        }

        // Verwijder een taak op basis van ID
        public async Task DeleteTask(Guid id)
        {
            var tasksCollection = _mongoDBService.GetTasksCollection();
            var task = await tasksCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
            if (task != null)
            {
                await tasksCollection.DeleteOneAsync(t => t.Id == id);
                Console.WriteLine($"Taak verwijderd: {task.Title} (ID: {task.Id})");
            }
        }

        // Verwijder taken gekoppeld aan een specifieke gebruiker
        public async Task DeleteTasksByUserId(Guid userId)
        {
            var tasksCollection = _mongoDBService.GetTasksCollection();
            var tasksToDelete = await tasksCollection.Find(t => t.Comments.Any(c => c.UserId == userId)).ToListAsync();
            foreach (var task in tasksToDelete)
            {
                await tasksCollection.DeleteOneAsync(t => t.Id == task.Id);
                Console.WriteLine($"Taak verwijderd: {task.Title} (ID: {task.Id}) gekoppeld aan UserId: {userId}");
            }
        }

        // Update een taak
        public async Task UpdateTask(TaskModel updatedTask)
        {
            var tasksCollection = _mongoDBService.GetTasksCollection();
            var task = await tasksCollection.Find(t => t.Id == updatedTask.Id).FirstOrDefaultAsync();
            if (task != null)
            {
                task.Title = updatedTask.Title;
                task.Description = updatedTask.Description;
                task.AssignedUser = updatedTask.AssignedUser;
                task.IsCompleted = updatedTask.IsCompleted;
                task.CreatedAt = updatedTask.CreatedAt; // Zorg ervoor dat de createdAt behouden blijft
                await tasksCollection.ReplaceOneAsync(t => t.Id == updatedTask.Id, task);
                Console.WriteLine($"Taak bijgewerkt: {task.Title} (ID: {task.Id})");
            }
        }

        // Voeg een comment toe aan een taak
        public async Task AddCommentToTask(Guid taskId, Comment comment)
        {
            var tasksCollection = _mongoDBService.GetTasksCollection();
            var task = await tasksCollection.Find(t => t.Id == taskId).FirstOrDefaultAsync();
            if (task != null)
            {
                comment.Id = Guid.NewGuid(); // Genereer een unieke ID voor de comment
                comment.TaskId = taskId;
                comment.CreatedAt = DateTime.UtcNow;

                var commentsCollection = _mongoDBService.GetCommentsCollection();
                await commentsCollection.InsertOneAsync(comment);
                Console.WriteLine($"Comment toegevoegd aan taak: {task.Title} (Task ID: {task.Id})");
            }
        }

        // Verkrijg comments voor een taak
        public async Task<IEnumerable<Comment>> GetCommentsByTaskId(Guid taskId)
        {
            var commentsCollection = _mongoDBService.GetCommentsCollection();
            return await commentsCollection.Find(c => c.TaskId == taskId).ToListAsync();
        }

        // Verkrijg taken die reacties bevatten van een specifieke gebruiker
        public async Task<IEnumerable<TaskModel>> GetTasksByUserIdInComments(Guid userId)
        {
            var tasksCollection = _mongoDBService.GetTasksCollection();
            var tasks = await tasksCollection.Find(t => t.Comments.Any(c => c.UserId == userId)).ToListAsync();
            return tasks;
        }
    }
}
