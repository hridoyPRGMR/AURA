using Core.Enums;


namespace Core.Models
{
    public class TaskStep : AuditedEntity
    {
        public long TaskItemId { get; private set; }
        public int Order { get; private set; }
        public string Instruction { get; private set; } = null!;
        public string? Result { get; private set; }
        public TaskStepStatus Status { get; private set; }
        public DateTime? StartedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public string? ErrorMessage { get; private set; }
        public TaskItem TaskItem { get; private set; } = null!;

        private TaskStep() { }


        public TaskStep(
            long id,
            long taskId,
            int order,
            string instruction) : base(id)
        {
            TaskItemId = taskId;
            Order = order;
            Instruction = instruction;
            Status = TaskStepStatus.Pending;
        }

        public void Start()
        {
            Status = TaskStepStatus.Running;
            StartedAt = DateTime.UtcNow;
        }

        public void Complete(string result)
        {
            Result = result;
            Status = TaskStepStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }

        public void Fail(string error)
        {
            ErrorMessage = error;
            Status = TaskStepStatus.Failed;
            CompletedAt = DateTime.UtcNow;
        }
    }
}