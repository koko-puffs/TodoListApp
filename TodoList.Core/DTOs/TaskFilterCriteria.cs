// --- File: TodoApp.Core/DTOs/TaskFilterCriteria.cs ---
// Data Transfer Object for passing filtering criteria to the service layer.
namespace TodoList.Core.DTOs // Can be in Core or a dedicated DTOs project/namespace
{
    /// <summary>
    /// Represents the criteria for filtering tasks.
    /// Nullable properties indicate that the criterion should not be applied if null.
    /// </summary>
    public class TaskFilterCriteria
    {
        public string? Keyword { get; set; }
        public bool? CompletionStatus { get; set; }
        public DateTime? DueDateStart { get; set; }
        public DateTime? DueDateEnd { get; set; }
        public int? PriorityLevel { get; set; }
        // Can add more criteria like CreatedDate range, etc.
    }
}