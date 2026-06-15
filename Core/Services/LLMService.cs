using Core.IServices;

namespace Core.Services
{
    public class LLMService : ILLMService
    {
        public async Task<string> ExecuteTaskAsync(string prompt)
        {
            return $"LLLM task working correctly through rabbitmq.prompt: {prompt}";
        }
    }
}