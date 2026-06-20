using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Core.IServices;
using Microsoft.Extensions.Logging;
using Shared.Dtos;

namespace Core.Services
{
    internal class AiAgentCallingService(
        HttpClient httpClient) : IAiAgentCallingService
    {
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        public async Task<string> CallAgentAsync(string url, string prompt, int maxTokens = 128)
        {
            var payload = new
            {
                messages = new[] { new { role = "user", content = prompt } },
                n_predict = maxTokens
            };

            var content = new StringContent(JsonSerializer.Serialize(payload, _jsonOptions), Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            var respBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Agent call failed: {(int)response.StatusCode} {response.ReasonPhrase} - {respBody}");
            }

            return ExtractContentFromJson(respBody);
        }

        public async Task<string> CallAgentAsync(AiAgentDto agent, string prompt)
        {
            // Build payload using the OpenAI/llama "messages" schema
            var bodyPayload = new Dictionary<string, object?>
            {
                ["messages"] = new[] { new Dictionary<string, string> { ["role"] = "user", ["content"] = prompt } },
                ["n_predict"] = agent.MaxTokens,
                ["temperature"] = agent.Temperature
            };

            if (agent.AdditionalConfigs is { Count: > 0 })
            {
                foreach (var (key, value) in agent.AdditionalConfigs)
                {
                    bodyPayload[key] = value;
                }
            }

            var content = new StringContent(JsonSerializer.Serialize(bodyPayload, _jsonOptions), Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, agent.Url) { Content = content };

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            var respBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Agent call failed: {(int)response.StatusCode} {response.ReasonPhrase} - {respBody}");
            }

            return ExtractContentFromJson(respBody);
        }

        private static string ExtractContentFromJson(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // OpenAI-style: { choices: [ { message: { content: "" } } ] }
            if (root.TryGetProperty("choices", out var choices) && choices.ValueKind == JsonValueKind.Array && choices.GetArrayLength() > 0)
            {
                var first = choices[0];
                if (first.TryGetProperty("message", out var message) && message.TryGetProperty("content", out var contentElem))
                {
                    return contentElem.GetString() ?? string.Empty;
                }

                if (first.TryGetProperty("text", out var textElem))
                {
                    return textElem.GetString() ?? string.Empty;
                }
            }

            // Llama-server style: { content: "..." }
            if (root.TryGetProperty("content", out var contentField))
            {
                return contentField.GetString() ?? string.Empty;
            }

            // Fallback to stringifying the whole body
            return json ?? string.Empty;
        }
    }
}