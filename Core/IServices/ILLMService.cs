namespace Core.IServices
{
    public interface ILLMService
    {
        Task<string> ExecuteTaskAsync(string prompt);
    }
}