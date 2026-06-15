using Shared.Dtos;

namespace Core.IServices
{
    public interface IMessagePublisher
    {
        Task PublishAsync(
            string queue,
            TaskCreatedMessage message,
            CancellationToken cancellationToken = default
        );
    }

}