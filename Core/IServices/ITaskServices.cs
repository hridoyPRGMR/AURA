using Shared.Dtos;

namespace Core.IServices
{
    public interface ITaskService
    {
        Task CreateTask(TaskItemCreateDto input);
    }
}