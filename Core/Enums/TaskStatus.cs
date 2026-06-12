using System.Collections.Frozen;

namespace Core.Enums
{
    public enum TaskItemStatus
    {
        Pending,
        Running,
        Done
    }

    public static class TaskItemStatusLookUp
    {
        public static readonly FrozenDictionary<TaskItemStatus,string>Labels = 
            new Dictionary<TaskItemStatus,string>
            {
                {TaskItemStatus.Pending, "Pending"},
                {TaskItemStatus.Running, "Running"},
                {TaskItemStatus.Pending, "Archived"},

            }.ToFrozenDictionary();
    }
}