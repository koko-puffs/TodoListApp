// --- File: TodoApp.Core/Interfaces/ITaskRepository.cs ---
// Defines the contract for data access operations related to tasks.
using TodoList.Core.Entities; // Need access to the TodoTask entity
using System.Collections.Generic;
using System.Threading.Tasks; // For asynchronous operations

namespace TodoList.Core.Interfaces
{
    /// <summary>
    /// Interface defining the required operations for persisting and retrieving TodoTask data.
    /// This abstraction allows decoupling the application logic from the specific data storage mechanism.
    /// </summary>
    public interface ITaskRepository
    {
        /// <summary>
        /// Retrieves a task by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the task to retrieve.</param>
        /// <returns>The found TodoTask, or null if no task with the specified ID exists.</returns>
        Task<TodoTask?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves all tasks.
        /// </summary>
        /// <returns>An enumerable collection of all TodoTasks.</returns>
        Task<IEnumerable<TodoTask>> GetAllAsync();

        /// <summary>
        /// Retrieves all tasks that are not completed.
        /// </summary>
        /// <returns>An enumerable collection of active (incomplete) TodoTasks.</returns>
        Task<IEnumerable<TodoTask>> GetActiveAsync();

        // Potentially add more specific retrieval methods if beneficial for performance
        // e.g., Task<IEnumerable<TodoTask>> GetTasksWithDueDateBeforeAsync(DateTime date);

        /// <summary>
        /// Adds a new task to the repository.
        /// The repository is typically responsible for assigning the ID.
        /// </summary>
        /// <param name="task">The task object to add.</param>
        /// <returns>The added task, potentially updated with its assigned ID and creation date.</returns>
        Task<TodoTask> AddAsync(TodoTask task);

        /// <summary>
        /// Updates an existing task in the repository.
        /// </summary>
        /// <param name="task">The task object with updated information.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="KeyNotFoundException">May be thrown by implementation if the task ID does not exist.</exception>
        Task UpdateAsync(TodoTask task);

        /// <summary>
        /// Deletes a task from the repository by its ID.
        /// </summary>
        /// <param name="id">The ID of the task to delete.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="KeyNotFoundException">May be thrown by implementation if the task ID does not exist.</exception>
        Task DeleteAsync(int id);

        /// <summary>
        /// Checks if a task with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID to check.</param>
        /// <returns>True if a task with the ID exists, false otherwise.</returns>
        Task<bool> ExistsAsync(int id);
    }
}