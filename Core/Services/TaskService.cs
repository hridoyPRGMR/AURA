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
        ILLMService llmService,
        IAiAgentCallingService aiAgentCalling,
        IMessagePublisher publisher) : ITaskService
    {

        public async Task CreateTask(TaskItemCreateDto input)
        {
            var task = new TaskItem(idGenerator.NewId(), input.UserPrompt);
            await taskRepository.AddAsync(task);
            await taskRepository.SaveChangesAsync();

            // Use selected agent metadata from static configuration
            var agent = AiAgentConfiguration.Planner;
            if (agent == null || string.IsNullOrEmpty(agent.Url))
                throw new InvalidOperationException("Selected AI agent is not configured in appsettings.json");

            var answer = await aiAgentCalling.CallAgentAsync(agent, input.UserPrompt);

            TaskCreatedMessage msg = new() { TaskId = task.Id };
            await publisher.PublishAsync(QueueNames.TaskCreated, msg);
        }

        public async Task ProcessTaskAsync(
            TaskCreatedMessage message,
            CancellationToken cancellationToken)
        {

            var task = await taskRepository.GetByIdAsync(message.TaskId, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"Task {message.TaskId} not found.");

            task.MarkRunning();
            await taskRepository.SaveChangesAsync(cancellationToken);

            try
            {
                var response =
                    await llmService.ExecuteTaskAsync(
                        task.UserPrompt);

                // TODO:
                // task.SetResult(response);

                task.MarkCompleted();
                await taskRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                task.MarkFailed(ex.Message);
                await taskRepository.SaveChangesAsync(cancellationToken);
                throw;
            }
        }
    }
}