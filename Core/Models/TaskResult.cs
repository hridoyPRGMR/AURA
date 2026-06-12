
namespace Core.Models
{
    public class TaskResult : AuditedEntity
    {
        public long TaskId { get; private set; }
        public string FinalOutput { get; private set; } = null!;
        public string? Summary { get; private set; }

        public TaskItem Task { get; private set; } = null!;

        private TaskResult(){}

        public TaskResult(
            long id,
            long taskId,
            string finalOutput,
            string? summary = null) : base(id)
        {
            TaskId = taskId;
            FinalOutput = finalOutput;
            Summary = summary;
        }

        public void UpdateOutput(string finalOutput, string? summary = null)
        {
            FinalOutput = finalOutput;
            Summary = summary;
        }
    }

}