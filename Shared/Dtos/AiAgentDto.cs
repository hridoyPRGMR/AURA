using System.Collections.Generic;

namespace Shared.Dtos
{
    public class AiAgentDto
    {
        public string Name { get; set; } = default!;
        public string Url { get; set; } = default!;
        public string ModelName { get; set; } = default!;
        public int MaxTokens { get; set; } = 128;
        public float Temperature { get; set; } = 0.7f;
        public Dictionary<string, object>? AdditionalConfigs { get; set; }
    }
}
