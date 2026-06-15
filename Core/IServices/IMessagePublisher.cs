namespace Core.IServices
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(
            string queue,
            T message,
            CancellationToken cancellationToken = default
        );
    }

}