using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.ApplicationDbContext
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
