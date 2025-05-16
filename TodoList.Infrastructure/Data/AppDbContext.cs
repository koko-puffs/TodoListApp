using Microsoft.EntityFrameworkCore;
using TodoList.Core.Entities;

namespace TodoList.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TodoTask> TodoTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // You can add any model configurations here if needed
            // For example, to configure the TodoItem entity:
            modelBuilder.Entity<TodoTask>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
                // EF Core 9 can infer string.Empty as default for required string if not nullable,
                // but explicit configuration is clearer.
                // entity.Property(e => e.Description).HasDefaultValue(string.Empty); // If desired for older EF or clarity
                entity.Property(e => e.Priority).HasConversion<string>(); // Store enum as string
            });
        }
    }
}
