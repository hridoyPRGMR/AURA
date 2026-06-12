using Core.Models;

namespace Core.IRepositories
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskItem task,CancellationToken ct = default);

        Task<TaskItem?> GetByIdAsync(long taskItemId, CancellationToken ct = default);

        Task<TaskItem?> GetWithStepsAsync(long taskId,CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}