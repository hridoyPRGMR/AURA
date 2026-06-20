using Microsoft.Extensions.Configuration;
using Shared.Dtos;

namespace Core.Common
{
    public static class AiAgentConfiguration
    {
        public static AiAgentDto? Planner { get; private set; }
        public static AiAgentDto? Qwen { get; private set; }
        public static AiAgentDto? Llama { get; private set; }
        public static AiAgentDto? Mistral { get; private set; }

        public static void Initialize(IConfiguration config)
        {
            Planner = LoadAgent(config, "Planner");
            Qwen = LoadAgent(config, "Qwen");
            Llama = LoadAgent(config, "Llama");
            Mistral = LoadAgent(config, "Mistral");

            // No default selected agent; developers should reference specific agents (Planner, Qwen, etc.)
        }

        private static AiAgentDto? LoadAgent(IConfiguration config, string agentKey)
        {
            var section = config.GetSection($"AiAgents:{agentKey}");
            var url = section["Url"];

            if (string.IsNullOrEmpty(url))
                return null;
            

            var maxTokens = 128;
            if (int.TryParse(section["MaxTokens"], out var parsedTokens))
                maxTokens = parsedTokens;
            
            var temperature = 0.7f;
            if (float.TryParse(section["Temperature"], out var parsedTemperature))
                temperature = parsedTemperature;

            return new AiAgentDto
            {
                Name = section["Name"] ?? agentKey,
                Url = url,
                ModelName = section["ModelName"] ?? string.Empty,
                MaxTokens = maxTokens,
                Temperature = temperature
            };
        }
    }
}