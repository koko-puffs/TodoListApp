using TodoList.Core.Entities;
using TodoList.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoList.Core.Interfaces
{
    public interface ITaskService
    {
        Task<TodoTask?> GetTaskByIdAsync(int id);
        Task<IEnumerable<TodoTask>> GetAllTasksAsync();
        Task<TodoTask> AddTaskAsync(string description, DateTime? dueDate = null, int priority = 0);
        Task DeleteTaskAsync(int id);
        Task UpdateTaskDescriptionAsync(int id, string newDescription);
        Task UpdateTaskDueDateAsync(int id, DateTime? newDueDate);

        Task MarkTaskCompleteAsync(int id);
        Task MarkTaskIncompleteAsync(int id);

        Task<IEnumerable<TodoTask>> GetActiveTasksAsync();
        Task<IEnumerable<TodoTask>> GetTasksByCriteriaAsync(TaskFilterCriteria criteria);
        Task<IEnumerable<TodoTask>> GetOverdueTasksAsync();
        Task UpdateTaskPriorityAsync(int id, int newPriority);
        Task RecalculatePriorityBasedOnRulesAsync(int id);

        Task DeleteTasksAsync(IEnumerable<int> ids);
        Task MarkTasksCompleteStatusAsync(IEnumerable<int> ids, bool isComplete);
    }
}