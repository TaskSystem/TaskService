using CommentService.DTO;
using CommentService.Models;
using CommentService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;

        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Comment>> GetComments()
        {
            return Ok(_commentRepository.GetAllComments());
        }

        [HttpGet("{id}")]
        public ActionResult<Comment> GetComment(Guid id)
        {
            var comment = _commentRepository.GetCommentById(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }

        [HttpPost]
        public IActionResult CreateComment([FromBody] Comment comment)
        {
            _commentRepository.AddComment(comment);
            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteComment(Guid id)
        {
            var comment = _commentRepository.GetCommentById(id);
            if (comment == null)
            {
                return NotFound();
            }

            _commentRepository.DeleteComment(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateComment(Guid id, [FromBody] CommentDTO updatedComment)
        {
            var existingComment = _commentRepository.GetCommentById(id);
            if (existingComment == null)
            {
                return NotFound();
            }

            // Zorg ervoor dat de ID gelijk blijft
            existingComment.Content = updatedComment.Content;
            existingComment.CreatedAt = updatedComment.CreatedAt;

            _commentRepository.UpdateComment(existingComment);

            return NoContent();
        }

        [HttpGet("task/{taskId}")]
        public ActionResult<IEnumerable<Comment>> GetCommentsByTaskId(Guid taskId)
        {
            var comments = _commentRepository.GetCommentsByTaskId(taskId);
            return Ok(comments);
        }
    }
}
