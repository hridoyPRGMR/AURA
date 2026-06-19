using System.Text;
using System.Text.Json;
using Core.IServices;
using Shared.Dtos;

namespace Core.Services
{
    internal class AiAgentCallingService(
        HttpClient httpClient) : IAiAgentCallingService
    {
        public async Task<string> CallAgentAsync(string url, string prompt, int maxTokens = 128)
        {
            var requestBody = new
            {
                prompt = prompt,
                n_predict = maxTokens
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.PostAsync(url,content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("content", out var contentElem))
            {
                return contentElem.GetString() ?? string.Empty;
            }

            return string.Empty;
        }

        public async Task<string> CallAgentAsync(AiAgentDto agent, string prompt)
        {
            var body = new Dictionary<string, object?>
            {
                ["prompt"] = prompt,
                ["n_predict"] = agent.MaxTokens,
                ["temperature"] = agent.Temperature,
            };

            if (agent.AdditionalConfigs is { Count: > 0 })
            {
                foreach (var (key, value) in agent.AdditionalConfigs)
                {
                    body[key] = value;
                }
            }

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.PostAsync(agent.Url, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("content", out var contentElem))
            {
                return contentElem.GetString() ?? string.Empty;
            }

            return string.Empty;
        }
    }
}