using TodoList.Core.Interfaces;
using TodoList.Core.Entities;
using TodoList.Core.DTOs;

namespace TodoList.Core.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        }

        public async Task<TodoTask> AddTaskAsync(string description, DateTime? dueDate = null, int priority = 0)
        {
            var newTask = new TodoTask(description)
            {
                DueDate = dueDate?.ToUniversalTime(),
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
            task.Description = newDescription;
            await _taskRepository.UpdateAsync(task);
        }

         public async Task UpdateTaskDueDateAsync(int id, DateTime? newDueDate)
        {
            var task = await GetTaskOrThrowNotFoundAsync(id);
            var newUtcDate = newDueDate?.ToUniversalTime();
            if (task.DueDate != newUtcDate)
            {
                task.DueDate = newUtcDate;
                await _taskRepository.UpdateAsync(task);
            }
        }

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

        public async Task<IEnumerable<TodoTask>> GetActiveTasksAsync()
        {
            return await _taskRepository.GetActiveAsync();
        }

        public async Task<IEnumerable<TodoTask>> GetTasksByCriteriaAsync(TaskFilterCriteria criteria)
        {
            var allTasks = await _taskRepository.GetAllAsync();

            IEnumerable<TodoTask> filteredTasks = allTasks;

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

            return filteredTasks.ToList();
        }

        public async Task<IEnumerable<TodoTask>> GetOverdueTasksAsync()
        {
            var activeTasks = await _taskRepository.GetActiveAsync();
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
            int calculatedPriority = 0;

            bool isOverdue = task.DueDate.HasValue && task.DueDate.Value < now && !task.IsCompleted;
            bool containsUrgentKeyword = task.Description.Contains("urgent", StringComparison.OrdinalIgnoreCase) ||
                                         task.Description.Contains("critical", StringComparison.OrdinalIgnoreCase);
            bool isDueSoon = task.DueDate.HasValue && task.DueDate.Value >= now && task.DueDate.Value < now.AddDays(2) && !task.IsCompleted;

            if (isOverdue && containsUrgentKeyword)
            {
                calculatedPriority = 2;
            }
            else if (isOverdue || containsUrgentKeyword)
            {
                calculatedPriority = 1;
            }
            else if (isDueSoon)
            {
                 calculatedPriority = 0;
            }
            else
            {
                 calculatedPriority = -1;
            }

            if (task.Priority != calculatedPriority)
            {
                task.Priority = calculatedPriority;
                await _taskRepository.UpdateAsync(task);
            }
        }

        public async Task DeleteTasksAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any()) return;

            List<Exception> errors = new List<Exception>();
            foreach (var id in ids.Distinct())
            {
                try
                {
                    if (await _taskRepository.ExistsAsync(id))
                    {
                        await _taskRepository.DeleteAsync(id);
                    }
                    else
                    {
                        errors.Add(new KeyNotFoundException($"Task with ID {id} not found for deletion during batch operation."));
                    }
                }
                catch (Exception ex) when (ex is not KeyNotFoundException)
                {
                    errors.Add(new InvalidOperationException($"Failed to delete task ID {id} during batch operation.", ex));
                }
            }

            if (errors.Any())
            {
                throw new AggregateException("One or more errors occurred during batch delete.", errors);
            }
        }

        public async Task MarkTasksCompleteStatusAsync(IEnumerable<int> ids, bool isComplete)
        {
            if (ids == null || !ids.Any()) return;

            List<Exception> errors = new List<Exception>();
            foreach (var id in ids.Distinct())
            {
                try
                {
                    if (isComplete)
                    {
                        await MarkTaskCompleteAsync(id);
                    }
                    else
                    {
                        await MarkTaskIncompleteAsync(id);
                    }
                }
                catch (KeyNotFoundException knfex)
                {
                     errors.Add(knfex);
                }
                catch (Exception ex)
                {
                    errors.Add(new InvalidOperationException($"Failed to update completion status for task ID {id}.", ex));
                }
            }
             if (errors.Any())
            {
                throw new AggregateException($"One or more errors occurred during batch status update.", errors);
            }
        }

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