using Microsoft.AspNetCore.Mvc;
using TaskService.DTO;
using TaskService.Model;
using TaskService.Repository;

namespace TaskService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoardController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public BoardController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        // Endpoint om alle boards op te halen
        [HttpGet]
        public IActionResult GetAllBoards()
        {
            var boards = _taskRepository.GetAllBoards();
            return Ok(boards);
        }

        // Endpoint om een specifiek board op te halen op basis van id
        [HttpGet("{id}")]
        public IActionResult GetBoardById(int id)
        {
            var board = _taskRepository.GetBoardById(id);
            if (board == null)
            {
                return NotFound();
            }
            return Ok(board);
        }

        // Endpoint om een board aan te maken
        [HttpPost]
        public IActionResult CreateBoard([FromBody] CreatedBoardDTO boardDto)
        {
            if (boardDto == null)
                return BadRequest();

            var board = new Board
            {
                Title = boardDto.Title,
                Description = boardDto.Description
            };

            _taskRepository.AddBoard(boardDto);

            return CreatedAtAction(nameof(GetBoardById), new { id = boardDto.Id }, boardDto);
        }

        // Endpoint om een board te updaten
        [HttpPut("{id}")]
        public IActionResult UpdateBoard(int id, [FromBody] UpdatedBoardDTO boardDto)
        {
            if (boardDto == null)
                return BadRequest();

            var board = _taskRepository.GetBoardById(id);
            if (board == null)
                return NotFound();

            _taskRepository.UpdateBoard(id, boardDto);
            return Ok("Board is updated.");
        }

        // Endpoint om een board te verwijderen
        [HttpDelete("{id}")]
        public IActionResult DeleteBoard(int id)
        {
            var board = _taskRepository.GetBoardById(id);
            if (board == null)
                return NotFound();

            _taskRepository.DeleteBoard(id);
            return NoContent();
        }
    }
}
