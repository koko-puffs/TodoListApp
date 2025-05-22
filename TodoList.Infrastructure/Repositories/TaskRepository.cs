using Microsoft.EntityFrameworkCore;
using TodoList.Core.Entities;
using TodoList.Core.Interfaces;
using TodoList.Infrastructure.Data;

namespace TodoList.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TodoTask?> GetByIdAsync(int id)
        {
            return await _context.TodoTasks.FindAsync(id);
        }

        public async Task<IEnumerable<TodoTask>> GetAllAsync()
        {
            return await _context.TodoTasks.ToListAsync();
        }

        public async Task<IEnumerable<TodoTask>> GetActiveAsync()
        {
            return await _context.TodoTasks.Where(t => !t.IsCompleted).ToListAsync();
        }

        public async Task<TodoTask> AddAsync(TodoTask task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            if (task.CreatedDate == default)
            {
                task.CreatedDate = DateTime.UtcNow;
            }
            
            _context.TodoTasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task UpdateAsync(TodoTask task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            var existingTask = await _context.TodoTasks.FindAsync(task.Id);
            if (existingTask == null)
            {
                throw new KeyNotFoundException($"Task with ID {task.Id} not found.");
            }

            _context.Entry(existingTask).CurrentValues.SetValues(task);
            
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var task = await _context.TodoTasks.FindAsync(id);
            if (task != null)
            {
                _context.TodoTasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Task with ID {id} not found for deletion.");
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.TodoTasks.AnyAsync(t => t.Id == id);
        }
    }
}
