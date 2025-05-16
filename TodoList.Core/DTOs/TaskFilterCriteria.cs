namespace TodoList.Core.DTOs 
{
    public class TaskFilterCriteria
    {
        public string? Keyword { get; set; }
        public bool? CompletionStatus { get; set; }
        public DateTime? DueDateStart { get; set; }
        public DateTime? DueDateEnd { get; set; }
        public int? PriorityLevel { get; set; }
    }
}