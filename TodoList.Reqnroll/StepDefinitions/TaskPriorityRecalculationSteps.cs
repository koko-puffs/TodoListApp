using Reqnroll;
using Moq;
using TodoList.Core.Entities;
using TodoList.Core.Services;
using TodoList.Core.Interfaces;

namespace TodoList.Reqnroll.StepDefinitions
{
    [Binding]
    public class TaskPriorityRecalculationSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private TodoTask? _task;
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly ITaskService _taskService;
        private Exception? _caughtException;
        private bool _databaseUpdatePerformed;

        private readonly DateTime _fixedCurrentDate;

        public TaskPriorityRecalculationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _fixedCurrentDate = DateTime.Now;
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _taskService = new TaskService(_taskRepositoryMock.Object);
        }

        private DateTime? ParseDueDateExpression(string dueDateExpression)
        {
            if (string.IsNullOrWhiteSpace(dueDateExpression) || dueDateExpression.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            if (dueDateExpression.Equals("YesterdayEndOfDay", StringComparison.OrdinalIgnoreCase))
            {
                return _fixedCurrentDate.Date.AddTicks(-1);
            }
            if (dueDateExpression.Equals("DueSoonUpperBoundary", StringComparison.OrdinalIgnoreCase))
            {
                return _fixedCurrentDate.Date.AddDays(2).AddTicks(-1);
            }

            var parts = dueDateExpression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 4 && parts[0].Equals("Today", StringComparison.OrdinalIgnoreCase) &&
                (parts[3].Equals("day", StringComparison.OrdinalIgnoreCase) || parts[3].Equals("days", StringComparison.OrdinalIgnoreCase)))
            {
                if (int.TryParse(parts[2], out int daysOffset))
                {
                    if (parts[1] == "-") return _fixedCurrentDate.AddDays(-daysOffset);
                    if (parts[1] == "+") return _fixedCurrentDate.AddDays(daysOffset);
                }
            }
            throw new ArgumentException($"Could not parse DueDateExpression: '{dueDateExpression}'. Supported formats: 'Today +/- X day(s)', 'null', 'YesterdayEndOfDay', 'DueSoonUpperBoundary'.");
        }

        [Given(@"a task with ID (.*)")]
        public void GivenATaskWithID(int taskId)
        {
            _task = new TodoTask { Id = taskId };
            _scenarioContext["TaskId"] = taskId;
            _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId))
                               .ReturnsAsync(() => _task);
        }

        [Given(@"the task description is ""(.*)""")]
        public void GivenTheTaskDescriptionIs(string description)
        {
            if (_task == null) throw new InvalidOperationException("Task not initialized. 'Given a task with ID' must be called first.");
            _task.Description = description;
        }

        [Given(@"the task description is null")]
        public void GivenTheTaskDescriptionIsNull()
        {
            if (_task == null) throw new InvalidOperationException("Task not initialized. 'Given a task with ID' must be called first.");
            _task.Description = null;
        }

        [Given(@"the task due date is ""(.*)""")]
        public void GivenTheTaskDueDateIs(string dueDateExpression)
        {
            if (_task == null) throw new InvalidOperationException("Task not initialized. 'Given a task with ID' must be called first.");
            _task.DueDate = ParseDueDateExpression(dueDateExpression);
        }

        [Given(@"the task is initially (completed|not completed)")]
        public void GivenTheTaskIsInitially(string completionStatus)
        {
            if (_task == null) throw new InvalidOperationException("Task not initialized. 'Given a task with ID' must be called first.");
            _task.IsCompleted = completionStatus == "completed";
        }

        [Given(@"the task has an initial priority of (.*)")]
        public void GivenTheTaskHasAnInitialPriorityOf(int initialPriority)
        {
            if (_task == null) throw new InvalidOperationException("Task not initialized. 'Given a task with ID' must be called first.");
            _task.Priority = initialPriority;
        }

        [Given(@"an invalid Task ID (.*)")]
        public void GivenAnInvalidTaskID(int invalidTaskId)
        {
            _scenarioContext["InvalidTaskId"] = invalidTaskId;
            _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(invalidTaskId))
                               .ReturnsAsync((TodoTask?)null);
        }

        [When(@"the priority recalculation process is triggered for the task")]
        public async Task WhenThePriorityRecalculationProcessIsTriggeredForTheTask()
        {
            if (_task == null) throw new InvalidOperationException("Task not initialized for 'When' step. Ensure 'Given a task with ID' was called.");
            _databaseUpdatePerformed = false;
            _caughtException = null;

            _taskRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<TodoTask>()))
                .Callback<TodoTask>(updatedTaskArg => {
                    _databaseUpdatePerformed = true;
                    if (_task != null && _task.Id == updatedTaskArg.Id)
                    {
                        _task.Priority = updatedTaskArg.Priority;
                    }
                })
                .Returns(Task.CompletedTask);

            try
            {
                await _taskService.RecalculatePriorityBasedOnRulesAsync(_task.Id);
            }
            catch (Exception ex)
            {
                _caughtException = ex.GetBaseException();
            }
        }

        [When(@"the priority recalculation process is triggered for the task with ID (.*)")]
        public async Task WhenThePriorityRecalculationProcessIsTriggeredForTheTaskWithID(int taskId)
        {
            _databaseUpdatePerformed = false;
            _caughtException = null;

            _taskRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<TodoTask>()))
                .Callback(() => _databaseUpdatePerformed = true)
                .Returns(Task.CompletedTask);
            
            try
            {
                await _taskService.RecalculatePriorityBasedOnRulesAsync(taskId);
            }
            catch (Exception ex)
            {
                _caughtException = ex.GetBaseException();
            }
        }

        [Then(@"the new priority of the task should be (.*)")]
        public void ThenTheNewPriorityOfTheTaskShouldBe(int expectedPriority)
        {
            Assert.Null(_caughtException);
            Assert.NotNull(_task);
            if (_task != null)
            {
                Assert.Equal(expectedPriority, _task.Priority);
            }
        }

        [Then(@"the database update should be (performed|skipped)")]
        public void ThenTheDatabaseUpdateShouldBe(string updateStatus)
        {
            bool expectedUpdate = updateStatus == "performed";
            Assert.Equal(expectedUpdate, _databaseUpdatePerformed);
        }

        [Then(@"an exception should be thrown")]
        public void ThenAnExceptionShouldBeThrown()
        {
            Assert.NotNull(_caughtException);
        }

        [Then(@"an exception should be thrown indicating ""(.*)""")]
        public void ThenAnExceptionShouldBeThrownIndicating(string expectedMessagePart)
        {
            Assert.NotNull(_caughtException);
            if (_caughtException != null)
            {
                if (expectedMessagePart.Equals("Task not found", StringComparison.OrdinalIgnoreCase))
                {
                    var argException = Assert.IsType<ArgumentException>(_caughtException);
                    Assert.Contains("Task not found", argException.Message, StringComparison.OrdinalIgnoreCase);
                }
                else if (expectedMessagePart.Equals("Description cannot be null when priority is critical", StringComparison.OrdinalIgnoreCase))
                {
                    var argNullException = Assert.IsType<ArgumentNullException>(_caughtException);
                    Assert.Equal("Description", argNullException.ParamName);
                    Assert.Contains("Description cannot be null", argNullException.Message, StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }
}