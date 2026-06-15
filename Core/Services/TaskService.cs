using Core.Common;
using Core.IRepositories;
using Core.IServices;
using Core.Models;
using Shared.Dtos;

namespace Core.Services
{
    public class TaskService(
        IIdGenerator idGenerator,
        ITaskRepository taskRepository,
        IMessagePublisher publisher) : ITaskService
    {
        private readonly IIdGenerator _idGenerator = idGenerator;
            private readonly ITaskRepository _taskRepository = taskRepository;
        private readonly IMessagePublisher _publisher = publisher;

        public async Task CreateTask(TaskItemCreateDto input)
        {
            var task = new TaskItem(_idGenerator.NewId(), input.UserPrompt);
                await _taskRepository.AddAsync(task);
                await _taskRepository.SaveChangesAsync();

            var msg = new { TaskId = task.Id };
            await _publisher.PublishAsync(QueueNames.TaskCreated, msg);
        }
    }
}