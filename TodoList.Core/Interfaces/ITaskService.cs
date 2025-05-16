// --- File: TodoApp.Core/Interfaces/ITaskService.cs ---
// Defines the contract for the application's business logic related to tasks.
using TodoList.Core.Entities;
using TodoList.Core.DTOs; // Reference the DTO namespace
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoList.Core.Interfaces
{
    /// <summary>
    /// Interface defining the application logic (use cases) for managing To-Do tasks.
    /// This layer orchestrates calls to the repository and implements business rules.
    /// </summary>
    public interface ITaskService
    {
        // --- Basic CRUD ---
        Task<TodoTask?> GetTaskByIdAsync(int id);
        Task<IEnumerable<TodoTask>> GetAllTasksAsync();
        Task<TodoTask> AddTaskAsync(string description, DateTime? dueDate = null, int priority = 0);
        Task DeleteTaskAsync(int id);
        Task UpdateTaskDescriptionAsync(int id, string newDescription);
        Task UpdateTaskDueDateAsync(int id, DateTime? newDueDate);

        // --- Status Updates ---
        Task MarkTaskCompleteAsync(int id);
        Task MarkTaskIncompleteAsync(int id);

        // --- Advanced Features ---
        Task<IEnumerable<TodoTask>> GetActiveTasksAsync();
        Task<IEnumerable<TodoTask>> GetTasksByCriteriaAsync(TaskFilterCriteria criteria);
        Task<IEnumerable<TodoTask>> GetOverdueTasksAsync();
        Task UpdateTaskPriorityAsync(int id, int newPriority); // Allow direct priority setting
        Task RecalculatePriorityBasedOnRulesAsync(int id); // Rule-based priority update

        // --- Batch Operations ---
        Task DeleteTasksAsync(IEnumerable<int> ids);
        Task MarkTasksCompleteStatusAsync(IEnumerable<int> ids, bool isComplete);
    }
}