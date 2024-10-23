using CommentService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CommentService.AppDBContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Comment> Comments { get; set; }
    }
}
