namespace TodoList.Core.Entities
{
    public class TodoTask
    {
        public int Id { get; set; }

        private string? _description;
        public string? Description
        {
            get => _description;
            set
            {
                if (value != null && string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Description, if provided, cannot be empty or whitespace.", nameof(Description));
                _description = value;
            }
        }

        public bool IsCompleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? DueDate { get; set; }

        public int Priority { get; set; }

        public TodoTask(string? description)
        {
            Description = description;
            IsCompleted = false;
            CreatedDate = DateTime.UtcNow;
            Priority = 0;
        }

        public TodoTask()
        {
             _description = "Please Update"; 
             CreatedDate = DateTime.UtcNow;
        }
    }
}