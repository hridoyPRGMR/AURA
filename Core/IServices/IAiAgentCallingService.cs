using Shared.Dtos;

namespace Core.IServices
{
    public interface IAiAgentCallingService
    {
        Task<string> CallAgentAsync(string baseUrl, string prompt, int maxTokens = 128);
        Task<string> CallAgentAsync(AiAgentDto agent, string prompt);
    }
}