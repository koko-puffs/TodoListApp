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
            
            // Ensure CreatedDate is set if not already provided by the caller (though typically service layer might handle this)
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

            // Update the properties of the tracked entity.
            // This approach ensures that only properties present in TodoTask are updated
            // and respects the entity's internal logic if any (e.g., setters).
            _context.Entry(existingTask).CurrentValues.SetValues(task);
            
            // If you want to handle concurrency, you might need to handle DbUpdateConcurrencyException here
            // or ensure the 'task' object passed in has a row version/timestamp if used.
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
                // The interface doc comment says: "May be thrown by implementation if the task ID does not exist."
                // So, throwing an exception is consistent.
                throw new KeyNotFoundException($"Task with ID {id} not found for deletion.");
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.TodoTasks.AnyAsync(t => t.Id == id);
        }
    }
}
