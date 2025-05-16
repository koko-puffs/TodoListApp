using TodoList.Core.Entities;

namespace TodoList.Core.Interfaces
{
    public interface ITaskRepository
    {
        Task<TodoTask?> GetByIdAsync(int id);

        Task<IEnumerable<TodoTask>> GetAllAsync();

        Task<IEnumerable<TodoTask>> GetActiveAsync();

        Task<TodoTask> AddAsync(TodoTask task);

        Task UpdateAsync(TodoTask task);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}