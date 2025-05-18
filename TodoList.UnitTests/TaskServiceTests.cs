using Moq;
using TodoList.Core.Entities;
using TodoList.Core.Interfaces;
using TodoList.Core.Services;

namespace TodoList.UnitTests // Adjusted namespace
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepository;
        private readonly ITaskService _taskService;

        public TaskServiceTests()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _taskService = new TaskService(_mockTaskRepository.Object);
        }

        [Theory]
        // Test Case Data: Description, DueDayOffset (from _fixedNow.Date), IsCompleted, InitialPriority, ExpectedPriority, UpdateOccurs
        [InlineData("urgent task", -1, false, 0, 2, true)]
        [InlineData("critical task", -1, false, 2, 2, false)]
        [InlineData("urgent old task", -1, true, 0, 1, true)]
        [InlineData("critical future task", 5, false, 0, 1, true)]
        [InlineData("normal overdue task", -1, false, 0, 1, true)]
        [InlineData("normal task due soon", 1, false, -1, 0, true)]
        [InlineData("normal task far future", 5, false, 0, -1, true)]
        [InlineData("completed task due soon", 1, true, 0, -1, true)]
        [InlineData("task no due date", null, false, 0, -1, true)]
        [InlineData("overdue task", -1, false, 0, 1, true)]

        public async Task RecalculatePriorityBasedOnRulesAsync_ShouldSetCorrectPriority_And_UpdateIfNeeded(
            string description,
            int? dueDayOffset,
            bool isCompleted,
            int initialPriority,
            int expectedPriority,
            bool updateOccurs)
        {
            // Arrange
            var taskId = 1;
            DateTime currentDate = DateTime.Now;
            DateTime? dueDate = dueDayOffset.HasValue ? currentDate.AddDays(dueDayOffset.Value) : (DateTime?)null;

            var task = new TodoTask
            {
                Id = taskId,
                Description = description,
                DueDate = dueDate,
                IsCompleted = isCompleted,
                Priority = initialPriority
            };

            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(task);

            // Act
            await _taskService.RecalculatePriorityBasedOnRulesAsync(taskId);

            // Assert
            Assert.Equal(expectedPriority, task.Priority);

            if (updateOccurs)
            {
                _mockTaskRepository.Verify(repo => repo.UpdateAsync(task), Times.Once);
            }
            else
            {
                _mockTaskRepository.Verify(repo => repo.UpdateAsync(It.IsAny<TodoTask>()), Times.Never);
            }
        }

        [Fact]
        public async Task RecalculatePriorityBasedOnRulesAsync_ShouldThrow_WhenTaskNotFound()
        {
            // Arrange
            var taskId = 999;
            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync((TodoTask)null!);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _taskService.RecalculatePriorityBasedOnRulesAsync(taskId));
        }
    }
}
