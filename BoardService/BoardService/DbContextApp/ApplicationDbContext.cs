using BoardService.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardService.DbContextApp
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {

        }

        public DbSet<BoardModel> Boards { get; set; }

        
    }
}
