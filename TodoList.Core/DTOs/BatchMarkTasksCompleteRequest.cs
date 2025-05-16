using System.ComponentModel.DataAnnotations;

namespace TodoList.Core.DTOs
{
    public class BatchMarkTasksCompleteRequest
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one task ID must be provided for batch update.")]
        public IEnumerable<int> Ids { get; set; } = new List<int>();

        [Required]
        public bool IsComplete { get; set; }
    }
}
