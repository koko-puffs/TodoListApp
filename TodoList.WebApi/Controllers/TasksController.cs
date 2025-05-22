using Microsoft.AspNetCore.Mvc;
using TodoList.Core.Entities;
using TodoList.Core.Interfaces;
using TodoList.Core.DTOs;

namespace TodoList.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoTask>>> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        // GET: api/Tasks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoTask>> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound($"Task with ID {id} not found.");
            }
            return Ok(task);
        }

        // POST: api/Tasks
        [HttpPost]
        public async Task<ActionResult<TodoTask>> CreateTask([FromBody] CreateTaskRequest requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdTask = await _taskService.AddTaskAsync(requestDto.Description, requestDto.DueDate, requestDto.Priority);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        // PUT: api/Tasks/{id}/description
        [HttpPut("{id}/description")]
        public async Task<IActionResult> UpdateTaskDescription(int id, [FromBody] UpdateTaskDescriptionRequest requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _taskService.UpdateTaskDescriptionAsync(id, requestDto.Description);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Tasks/{id}/duedate
        [HttpPut("{id}/duedate")]
        public async Task<IActionResult> UpdateTaskDueDate(int id, [FromBody] UpdateTaskDueDateRequest requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _taskService.UpdateTaskDueDateAsync(id, requestDto.DueDate);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // PUT: api/Tasks/{id}/priority
        [HttpPut("{id}/priority")]
        public async Task<IActionResult> UpdateTaskPriority(int id, [FromBody] UpdateTaskPriorityRequest requestDto)
        {
             if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _taskService.UpdateTaskPriorityAsync(id, requestDto.Priority);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // PUT: api/Tasks/{id}/complete
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> MarkTaskComplete(int id)
        {
            try
            {
                await _taskService.MarkTaskCompleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // PUT: api/Tasks/{id}/incomplete
        [HttpPut("{id}/incomplete")]
        public async Task<IActionResult> MarkTaskIncomplete(int id)
        {
            try
            {
                await _taskService.MarkTaskIncompleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/Tasks/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<TodoTask>>> GetActiveTasks()
        {
            var tasks = await _taskService.GetActiveTasksAsync();
            return Ok(tasks);
        }

        // POST: api/Tasks/filter
        [HttpPost("filter")]
        public async Task<ActionResult<IEnumerable<TodoTask>>> GetTasksByCriteria([FromBody] TaskFilterCriteria criteria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tasks = await _taskService.GetTasksByCriteriaAsync(criteria);
            return Ok(tasks);
        }

        // GET: api/Tasks/overdue
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<TodoTask>>> GetOverdueTasks()
        {
            var tasks = await _taskService.GetOverdueTasksAsync();
            return Ok(tasks);
        }

        // PUT: api/Tasks/{id}/recalculate-priority
        [HttpPut("{id}/recalculate-priority")]
        public async Task<IActionResult> RecalculatePriority(int id)
        {
            try
            {
                await _taskService.RecalculatePriorityBasedOnRulesAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/Tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                await _taskService.DeleteTaskAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/Tasks/batch-delete
        [HttpPost("batch-delete")]
        public async Task<IActionResult> BatchDeleteTasks([FromBody] BatchDeleteTasksRequest requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _taskService.DeleteTasksAsync(requestDto.Ids);
            return Ok("Batch delete operation attempted.");
        }

        // POST: api/Tasks/batch-mark-complete
        [HttpPost("batch-mark-complete")]
        public async Task<IActionResult> BatchMarkTasksComplete([FromBody] BatchMarkTasksCompleteRequest requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _taskService.MarkTasksCompleteStatusAsync(requestDto.Ids, requestDto.IsComplete);
            return Ok($"Batch mark as '{(requestDto.IsComplete ? "complete" : "incomplete")}' operation attempted.");
        }
    }
}
