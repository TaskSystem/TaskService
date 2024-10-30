using Microsoft.AspNetCore.Mvc;
using TaskService.Model;
using TaskService.Repository;

namespace TaskService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly PublisherService _publisherService;
        private readonly HttpClient _httpClient;

        public TaskController(ITaskRepository taskRepository, PublisherService publisherService, HttpClient httpClient)
        {
            _taskRepository = taskRepository;
            _publisherService = publisherService;
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult GetAllTasks()
        {
            var tasks = _taskRepository.GetAllTasks();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public IActionResult GetTaskById(Guid id)
        {
            var task = _taskRepository.GetTaskById(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        

        [HttpPost]
        public IActionResult CreateTask([FromBody] TaskModel task)
        {
            if (task == null)
                return BadRequest();
            _taskRepository.AddTask(task);



            // Maak het evenement aan
            var taskCreatedEvent = new TaskCreatedEvent
            {
                TaskName = task.Title,
                Email = task.Email
            };

            _publisherService.Publish(taskCreatedEvent);




            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(Guid id, [FromBody] TaskModel task)
        {
            var Task = _taskRepository.GetTaskById(id);
            if (task == null)
                return NotFound();
            task.Id = id;
            _taskRepository.UpdateTask(task);
            return Ok("This task is updated");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(Guid id)
        {
            var task = _taskRepository.GetTaskById(id);
            if (task == null)
                return NotFound();

            _taskRepository.DeleteTask(id);
            return NoContent();
        }

        [HttpPatch("{id}/complete")]
        public IActionResult CompleteTask(Guid id)
        {
            var Task = _taskRepository.GetTaskById(id);
            if (Task == null)
                return NotFound();

            Task.IsCompleted = true;
            _taskRepository.UpdateTask(Task);
            return NoContent();
        }

        [HttpPatch("{id}/adduser")]
        public IActionResult AddUserToTask(Guid id, [FromBody] string username)
        {
            var task = _taskRepository.GetTaskById(id);
            if(task == null)
                return NotFound();

            task.AssignedUser = username;
            _taskRepository.UpdateTask(task);
            return NoContent();
        }

        [HttpPost("{taskId}/comments")]
        public IActionResult AddCommentToTask(Guid taskId, [FromBody] Comment comment)
        {
            _taskRepository.AddCommentToTask(taskId, comment);
            return CreatedAtAction(nameof(GetCommentsByTaskId), new { taskId = taskId }, comment);
        }

        [HttpGet("{taskId}/comments")]
        public IActionResult GetCommentsByTaskId(Guid taskId)
        {
            var comments = _taskRepository.GetCommentsByTaskId(taskId);
            return Ok(comments);
        }

    }
}
