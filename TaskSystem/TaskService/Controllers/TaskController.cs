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

        public TaskController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
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
            

    }
}
