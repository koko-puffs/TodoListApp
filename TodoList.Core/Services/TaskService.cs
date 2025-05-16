// --- File: TodoApp.Core/Services/TaskService.cs ---
// Implements the ITaskService interface, containing the application's business logic.
// Depends on ITaskRepository via Dependency Injection, making it testable.
using TodoList.Core.Interfaces; // Implements this interface, uses the other interface
using TodoList.Core.Entities; // Works with these entities
using TodoList.Core.DTOs;     // Uses filter DTO
using System;
using System.Collections.Generic;
using System.Linq; // Needed for filtering, etc.
using System.Threading.Tasks; // For async methods

namespace TodoList.Core.Services
{
    /// <summary>
    /// Provides the core application logic for managing To-Do tasks.
    /// Orchestrates operations using an ITaskRepository.
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        // Dependency Injection via Constructor
        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        }

        // --- Basic CRUD Implementations ---

        public async Task<TodoTask> AddTaskAsync(string description, DateTime? dueDate = null, int priority = 0)
        {
            // Validation is handled by the TodoTask constructor/setter for description
            var newTask = new TodoTask(description)
            {
                DueDate = dueDate?.ToUniversalTime(), // Store dates in UTC
                Priority = priority
            };
            return await _taskRepository.AddAsync(newTask);
        }

        public async Task DeleteTaskAsync(int id)
        {
            if (!await _taskRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Cannot delete: Task with ID {id} not found.");
            }
            await _taskRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<TodoTask>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllAsync();
        }

        public async Task<TodoTask?> GetTaskByIdAsync(int id)
        {
            return await _taskRepository.GetByIdAsync(id);
        }

        public async Task UpdateTaskDescriptionAsync(int id, string newDescription)
        {
            var task = await GetTaskOrThrowNotFoundAsync(id);
            // Validation handled by property setter
            task.Description = newDescription;
            await _taskRepository.UpdateAsync(task);
        }

         public async Task UpdateTaskDueDateAsync(int id, DateTime? newDueDate)
        {
            var task = await GetTaskOrThrowNotFoundAsync(id);
            var newUtcDate = newDueDate?.ToUniversalTime(); // Ensure UTC
            if (task.DueDate != newUtcDate)
            {
                task.DueDate = newUtcDate;
                await _taskRepository.UpdateAsync(task);
            }
        }

        // --- Status Update Implementations ---

        public async Task MarkTaskCompleteAsync(int id)
        {
            var task = await GetTaskOrThrowNotFoundAsync(id);
            if (!task.IsCompleted)
            {
                task.IsCompleted = true;
                await _taskRepository.UpdateAsync(task);
            }
        }

        public async Task MarkTaskIncompleteAsync(int id)
        {
            var task = await GetTaskOrThrowNotFoundAsync(id);
            if (task.IsCompleted)
            {
                task.IsCompleted = false;
                await _taskRepository.UpdateAsync(task);
            }
        }

        // --- Advanced Feature Implementations ---

        public async Task<IEnumerable<TodoTask>> GetActiveTasksAsync()
        {
            // Delegate directly if repository supports it efficiently
            return await _taskRepository.GetActiveAsync();
            // Alternative if repo doesn't have GetActiveAsync:
            // var allTasks = await _taskRepository.GetAllAsync();
            // return allTasks.Where(t => !t.IsCompleted);
        }

        public async Task<IEnumerable<TodoTask>> GetTasksByCriteriaAsync(TaskFilterCriteria criteria)
        {
            // Fetch all tasks - Optimization: If using a DB, build a dynamic query instead.
            var allTasks = await _taskRepository.GetAllAsync();

            // Apply filtering logic in memory
            IEnumerable<TodoTask> filteredTasks = allTasks; // Start with all

            if (!string.IsNullOrWhiteSpace(criteria.Keyword))
            {
                filteredTasks = filteredTasks.Where(t =>
                    t.Description.Contains(criteria.Keyword, StringComparison.OrdinalIgnoreCase));
            }
            if (criteria.CompletionStatus.HasValue)
            {
                filteredTasks = filteredTasks.Where(t => t.IsCompleted == criteria.CompletionStatus.Value);
            }
            if (criteria.DueDateStart.HasValue)
            {
                var startDateUtc = criteria.DueDateStart.Value.ToUniversalTime();
                filteredTasks = filteredTasks.Where(t => t.DueDate.HasValue && t.DueDate.Value >= startDateUtc);
            }
            if (criteria.DueDateEnd.HasValue)
            {
                 var endDateUtc = criteria.DueDateEnd.Value.ToUniversalTime();
                filteredTasks = filteredTasks.Where(t => t.DueDate.HasValue && t.DueDate.Value <= endDateUtc);
            }
            if (criteria.PriorityLevel.HasValue)
            {
                filteredTasks = filteredTasks.Where(t => t.Priority == criteria.PriorityLevel.Value);
            }

            return filteredTasks.ToList(); // Materialize the result
        }

        public async Task<IEnumerable<TodoTask>> GetOverdueTasksAsync()
        {
            var activeTasks = await _taskRepository.GetActiveAsync(); // More efficient
            var now = DateTime.UtcNow;
            return activeTasks.Where(t => t.DueDate.HasValue && t.DueDate.Value < now).ToList();
        }

        public async Task UpdateTaskPriorityAsync(int id, int newPriority)
        {
             var task = await GetTaskOrThrowNotFoundAsync(id);
             if(task.Priority != newPriority)
             {
                 task.Priority = newPriority;
                 await _taskRepository.UpdateAsync(task);
             }
        }

        public async Task RecalculatePriorityBasedOnRulesAsync(int id)
        {
            var task = await GetTaskOrThrowNotFoundAsync(id);
            var now = DateTime.UtcNow;
            int calculatedPriority = 0; // Default priority

            // --- Example Priority Rules (Complex Logic for White-Box Testing) ---
            bool isOverdue = task.DueDate.HasValue && task.DueDate.Value < now && !task.IsCompleted;
            bool containsUrgentKeyword = task.Description.Contains("urgent", StringComparison.OrdinalIgnoreCase) ||
                                         task.Description.Contains("critical", StringComparison.OrdinalIgnoreCase);
            bool isDueSoon = task.DueDate.HasValue && task.DueDate.Value >= now && task.DueDate.Value < now.AddDays(2) && !task.IsCompleted;

            if (isOverdue && containsUrgentKeyword)
            {
                calculatedPriority = 2; // Highest priority
            }
            else if (isOverdue || containsUrgentKeyword)
            {
                calculatedPriority = 1; // High priority
            }
            else if (isDueSoon)
            {
                 calculatedPriority = 0; // Normal, but maybe flag? (Could return more info)
            }
            else
            {
                 calculatedPriority = -1; // Low priority (example)
            }
            // --- End Example Priority Rules ---

            // Update only if changed
            if (task.Priority != calculatedPriority)
            {
                task.Priority = calculatedPriority;
                await _taskRepository.UpdateAsync(task);
            }
        }


        // --- Batch Operation Implementations ---

        public async Task DeleteTasksAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any()) return; // Nothing to do

            List<Exception> errors = new List<Exception>();
            foreach (var id in ids.Distinct()) // Process unique IDs
            {
                try
                {
                    // We need to check existence before deleting, as DeleteAsync might throw or do nothing
                    if (await _taskRepository.ExistsAsync(id))
                    {
                        await _taskRepository.DeleteAsync(id);
                    }
                    else
                    {
                        // Collect errors for non-existent tasks if strict behavior is needed
                        errors.Add(new KeyNotFoundException($"Task with ID {id} not found for deletion during batch operation."));
                    }
                }
                catch (Exception ex) when (ex is not KeyNotFoundException) // Catch unexpected errors
                {
                    // Log error and collect it
                    errors.Add(new InvalidOperationException($"Failed to delete task ID {id} during batch operation.", ex));
                }
            }

            if (errors.Any())
            {
                // Throw an aggregate exception summarizing the issues
                throw new AggregateException("One or more errors occurred during batch delete.", errors);
            }
        }

        public async Task MarkTasksCompleteStatusAsync(IEnumerable<int> ids, bool isComplete)
        {
            if (ids == null || !ids.Any()) return;

            List<Exception> errors = new List<Exception>();
            foreach (var id in ids.Distinct()) // Process unique IDs
            {
                try
                {
                    // Reuse single-item logic which includes checks and update logic
                    if (isComplete)
                    {
                        await MarkTaskCompleteAsync(id);
                    }
                    else
                    {
                        await MarkTaskIncompleteAsync(id);
                    }
                }
                catch (KeyNotFoundException knfex) // Expected error if ID doesn't exist
                {
                     errors.Add(knfex); // Collect the error
                }
                catch (Exception ex) // Catch unexpected errors
                {
                    errors.Add(new InvalidOperationException($"Failed to update completion status for task ID {id}.", ex));
                }
            }
             if (errors.Any())
            {
                throw new AggregateException($"One or more errors occurred during batch status update.", errors);
            }
        }

        // --- Private Helper Methods ---

        /// <summary>
        /// Retrieves a task by ID or throws a KeyNotFoundException if it doesn't exist.
        /// Reduces repetition in other service methods.
        /// </summary>
        private async Task<TodoTask> GetTaskOrThrowNotFoundAsync(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                throw new KeyNotFoundException($"Operation failed: Task with ID {id} not found.");
            }
            return task;
        }
    }
}