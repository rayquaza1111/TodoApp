using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Todo> Todos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Todo entity
            modelBuilder.Entity<Todo>()
                .Property(t => t.Title)
                .IsRequired();

            modelBuilder.Entity<Todo>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
} 