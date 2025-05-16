using System;
using System.ComponentModel.DataAnnotations;

namespace TodoList.Core.DTOs
{
    public class CreateTaskRequest
    {
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 200 characters.")]
        public string Description { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        public int Priority { get; set; } = 0; // Default priority
    }
}
