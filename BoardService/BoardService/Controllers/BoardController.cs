using BoardService.DbContextApp;
using BoardService.Dto;
using BoardService.Messaging;
using BoardService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoardService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private readonly BoardContext _context;
        //private readonly PublisherToRMQ _publisher;

        public BoardController(BoardContext context, PublisherToRMQ publisherToRMQ)
        {
            _context = context;
            //_publisher = publisherToRMQ; // Instantie van de publisher
        }


        [HttpPost]
        public async Task<IActionResult> CreateBoard(CreatedBoardDTO boardDTO)
        {
            var board = new BoardModel
            {
                Title = boardDTO.Title,
                Description = boardDTO.Description
            };

            _context.Boards.Add(board);
            await _context.SaveChangesAsync();

            // Publiceer de board-informatie naar RabbitMQ
            //_publisher.Publish(board);

            return CreatedAtAction(nameof(GetBoardById), new { id = board.Id }, board);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardModel>>> GetBoard()
        {
            return await _context.Boards.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BoardModel>> GetBoardById(int id)
        {
            var board = await _context.Boards.FindAsync(id);

            if (board == null)
            {
                return NotFound();
            }

            return board;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBoard(int id, UpdatedBoardDTO boardDTO)
        {
            var board = await _context.Boards.FindAsync(id);

            if(board == null)
            {
                return NotFound();
            }

            board.Title = boardDTO.Title;
            board.Description = boardDTO.Description;
            board.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoard(int id)
        {
            var board = await _context.Boards.FindAsync(id);

            if (board == null)
            {
                return NotFound();
            }

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
