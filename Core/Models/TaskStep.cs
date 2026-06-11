namespace AURA.Core.Models
{
    public class TaskStep
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public required string Instruction { get; set; }
        public string? Result { get; set; }
        public int Order { get; set; }
    }
}