using AURA.Core.Enums.TaskItemStatus;

namespace AURA.Core.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public required string UserPrompt { get; set; } 
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}