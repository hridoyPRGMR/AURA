using Core.Enums;

namespace Core.Models
{
    public class TaskItem : AuditedEntity
    {
        public string UserPrompt { get; private set; } 
        public TaskItemStatus Status { get; private set; } = TaskItemStatus.Pending;
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

        public void MarkRunning()
        {
            Status = TaskItemStatus.Running;
        }

        public void MarkCompleted()
        {
            Status = TaskItemStatus.Done;
        }

        public void MarkFailed(string message)
        {
            Status = TaskItemStatus.Failed;
        }
    }
}