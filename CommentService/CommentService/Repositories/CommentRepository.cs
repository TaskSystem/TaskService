using CommentService.AppDBContext;
using CommentService.DTO;
using CommentService.Models;

namespace CommentService.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly List<Comment> _comments = new();

        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Comment> GetAllComments()
        {
            return _context.Comments.ToList();
        }

        public Comment GetCommentById(Guid id)
        {
            return _context.Comments.FirstOrDefault(c => c.Id == id);
        }

        public void AddComment(Comment comment)
        {
            comment.Id = Guid.NewGuid(); // Genereer een nieuwe GUID voor de comment
            comment.CreatedAt = DateTime.UtcNow; // Voeg de aanmaakdatum toe
            _context.Add(comment);
            _context.SaveChanges();
        }

        public void DeleteComment(Guid id)
        {
            var comment = GetCommentById(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }
        }

        public void UpdateComment(Comment comments)
        {
            _context.Comments.Update(comments);
            _context.SaveChanges();
        }

        public Comment GetCommentsByTaskId(Guid taskId)
        {
            return _context.Comments.FirstOrDefault(u => u.Id == taskId);
        }
    }
}
