using Shared.Dtos;

namespace Core.IServices
{
    public interface ITaskService
    {
        Task CreateTask(TaskItemCreateDto input);
        Task ProcessTaskAsync(TaskCreatedMessage message,CancellationToken cancellationToken);
    }
}