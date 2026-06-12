using Core.IRepositories;
using Core.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    internal sealed class TaskRepository(AuraDbContext db) : ITaskRepository
    {

        public async Task AddAsync(
            TaskItem task,
            CancellationToken ct = default)
        {
            await db.Tasks.AddAsync(task, ct);
        }

        public async Task<TaskItem?> GetByIdAsync(
            long taskId,
            CancellationToken ct = default)
        {
            return await db.Tasks
                .FirstOrDefaultAsync(
                    x => x.Id == taskId,
                    ct);
        }

        public async Task<TaskItem?> GetWithStepsAsync(
            long taskId,
            CancellationToken ct = default)
        {
            return await db.Tasks
                .Include(x => x.Steps)
                .Include(x => x.Result)
                .FirstOrDefaultAsync(
                    x => x.Id == taskId,
                    ct);
        }

        public Task SaveChangesAsync(
            CancellationToken ct = default)
        {
            return db.SaveChangesAsync(ct);
        }
    }
}