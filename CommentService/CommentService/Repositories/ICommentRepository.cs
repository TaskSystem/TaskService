using CommentService.DTO;
using CommentService.Models;

namespace CommentService.Repositories
{
    public interface ICommentRepository
    {
        IEnumerable<Comment> GetAllComments();
        Comment GetCommentById(Guid id);
        void AddComment(Comment comment);
        void DeleteComment(Guid id);
        void UpdateComment(Comment comment);
        Comment GetCommentsByTaskId(Guid taskId); // Voor het ophalen van comments per taak
    }
}
