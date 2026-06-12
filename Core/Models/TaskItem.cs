using Core.Enums;

namespace Core.Models
{
    public class TaskItem : AuditedEntity
    {
        public required string UserPrompt { get; set; } 
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
        public ICollection<TaskStep> Steps  {get; private set;} = []; 
        public TaskResult? Result { get; private set; }

        private TaskItem(){}

        internal TaskItem(
            long id,
            string prompt
            ) : base(id)
        {
            UserPrompt = prompt;            
        }

    }
}