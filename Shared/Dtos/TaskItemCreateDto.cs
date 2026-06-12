using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos
{
    public class TaskItemCreateDto
    {
        [Required,MaxLength(1000)]
        public string UserPrompt {get; init;} = default!; 
    }
    
}