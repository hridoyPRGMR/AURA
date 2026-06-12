using Core.IRepositories;
using Core.IServices;
using Core.Models;
using Shared.Dtos;

namespace Core.Services
{
    public class TaskService(
        IIdGenerator idGenerator,
        ITaskRepository taskRepository
    ) : ITaskService
    {
        public async Task CreateTask(TaskItemCreateDto input)
        {
            TaskItem task = new(idGenerator.NewId(), input.UserPrompt);
            await taskRepository.AddAsync(task);
            await taskRepository.SaveChangesAsync();
        }
    }
}