using System.ComponentModel.DataAnnotations;

namespace TodoList.Core.DTOs
{
    public class BatchDeleteTasksRequest
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one task ID must be provided for batch deletion.")]
        public IEnumerable<int> Ids { get; set; } = new List<int>();
    }
}
