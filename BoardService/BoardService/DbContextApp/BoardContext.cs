using Microsoft.EntityFrameworkCore;
using BoardService.Models;

namespace BoardService.DbContextApp
{
    public class BoardContext : DbContext
    {
        public DbSet<BoardModel> Boards { get; set; }

        public BoardContext(DbContextOptions<BoardContext> options) : base(options) { }

    }
}
