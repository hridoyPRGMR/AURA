using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AuraDbContext(DbContextOptions<AuraDbContext> options) : DbContext(options)
    {
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<TaskStep> TaskSteps => Set<TaskStep>();
         public DbSet<TaskResult> TaskResults => Set<TaskResult>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AuraDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}