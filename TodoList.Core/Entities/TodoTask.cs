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
                // Allow null, but if not null, it cannot be empty or whitespace.
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
             // This assignment is fine if it's meant to be a non-null default
             // when the parameterless constructor is used. It's not null or whitespace.
             _description = "Temporary Description - Please Update"; 
             // If Description should be null by default with this constructor, then: 
             // _description = null;
             CreatedDate = DateTime.UtcNow;
        }
    }
}