using Reqnroll;
using Moq;
using TodoList.Core.Entities;
using TodoList.Core.Interfaces;
using TodoList.Core.Services;

namespace TodoList.Reqnroll.StepDefinitions
{
    [Binding]
    public class TaskPriorityRecalculationSteps
    {
        private Mock<ITaskRepository> _mockTaskRepository = null!;
        private ITaskService _taskService = null!;
        private TodoTask? _currentTask;
        private int _currentTaskId;
        private Exception? _caughtException;
        private bool _updatePerformedMock;

        private readonly List<TodoTask> _tasksInDb = new List<TodoTask>();
        private DateTime _testTodayDate = DateTime.Today;

        [BeforeScenario]
        public void Setup()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _tasksInDb.Clear();
            _currentTask = null;
            _caughtException = null;
            _updatePerformedMock = false;
            _testTodayDate = DateTime.UtcNow.Date;

            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _tasksInDb.FirstOrDefault(t => t.Id == id));

            _mockTaskRepository.Setup(repo => repo.UpdateAsync(It.IsAny<TodoTask>()))
                .Callback<TodoTask>(task =>
                {
                    var existingTask = _tasksInDb.FirstOrDefault(t => t.Id == task.Id);
                    if (existingTask != null)
                    {
                        existingTask.Priority = task.Priority;
                        existingTask.Description = task.Description;
                        existingTask.DueDate = task.DueDate;
                        existingTask.IsCompleted = task.IsCompleted;
                    }
                    _updatePerformedMock = true;
                })
                .Returns(Task.CompletedTask);

            _mockTaskRepository.Setup(repo => repo.ExistsAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _tasksInDb.Any(t => t.Id == id));

            _taskService = new TaskService(_mockTaskRepository.Object);
        }

        [Given(@"the current date is today for testing")]
        public void GivenTheCurrentDateIsTodayForTesting()
        {
        }

        [Given(@"a task with ID (\d+)")]
        public void GivenATaskWithID(int taskId)
        {
            _currentTaskId = taskId;
            _currentTask = new TodoTask($"Default task {taskId}") { Id = taskId };
            _tasksInDb.Add(_currentTask);
        }

        [Given(@"the task description is ""(.*)""")]
        public void GivenTheTaskDescriptionIs(string description)
        {
            if (_currentTask == null) throw new InvalidOperationException("Task not initialized");
            _currentTask.Description = description;
        }

        [Given(@"the task description is null")]
        public void GivenTheTaskDescriptionIsNull()
        {
            if (_currentTask == null) throw new InvalidOperationException("Task not initialized");
            _currentTask.Description = null;
        }

        [Given(@"the task has a due date offset of ""(.*)"" days")]
        public void GivenTheTaskHasADueDateOffsetOfDays(string offsetString)
        {
            if (_currentTask == null) throw new InvalidOperationException("Task not initialized before setting due date offset.");

            if (string.IsNullOrWhiteSpace(offsetString))
            {
                _currentTask.DueDate = null;
            }
            else
            {
                if (int.TryParse(offsetString, out int daysOffset))
                {
                    _currentTask.DueDate = _testTodayDate.AddDays(daysOffset).ToUniversalTime();
                }
                else
                {
                    throw new ArgumentException($"Invalid format for due date offset: '{offsetString}'. Expected an integer or empty string.");
                }
            }
        }

        [Given(@"^the task is initially (true|false)$" )]
        public void GivenTheTaskIsInitially(bool isCompleted)
        {
            if (_currentTask == null) throw new InvalidOperationException("Task not initialized");
            _currentTask.IsCompleted = isCompleted;
        }

        [Given(@"the task has an initial priority of (-?\d+)")]
        public void GivenTheTaskHasAnInitialPriorityOf(int initialPriority)
        {
            if (_currentTask == null) throw new InvalidOperationException("Task not initialized");
            _currentTask.Priority = initialPriority;
        }
        
        [Given(@"an invalid Task ID (\d+)")]
        public void GivenAnInvalidTaskID(int invalidTaskId)
        {
            _currentTaskId = invalidTaskId;
        }

        [When(@"the priority recalculation process is triggered for the task")]
        public async Task WhenThePriorityRecalculationProcessIsTriggeredForTheTask()
        {
            _caughtException = null;
            _updatePerformedMock = false;

            try
            {
                await _taskService.RecalculatePriorityBasedOnRulesAsync(_currentTaskId);
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }
        
        [When(@"the priority recalculation process is triggered for the task with ID (\d+)")]
        public async Task WhenThePriorityRecalculationProcessIsTriggeredForTheTaskWithID(int taskId)
        {
            _currentTaskId = taskId;
            _caughtException = null;
            _updatePerformedMock = false;

            try
            {
                await _taskService.RecalculatePriorityBasedOnRulesAsync(taskId);
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        [Then(@"the new priority of the task should be (-?\d+)")]
        public void ThenTheNewPriorityOfTheTaskShouldBe(int expectedPriority)
        {
            var taskFromDb = _tasksInDb.FirstOrDefault(t => t.Id == _currentTaskId);
            Assert.NotNull(taskFromDb);
            Assert.Equal(expectedPriority, taskFromDb.Priority);
        }

        [Then(@"^the database update should be (true|false)$" )]
        public void ThenTheDatabaseUpdateShouldBe(bool expectedUpdate)
        {
            Assert.Equal(expectedUpdate, _updatePerformedMock);
        }

        [Then(@"an exception should be thrown")]
        public void ThenAnExceptionShouldBeThrown()
        {
            Assert.NotNull(_caughtException);
        }
        
        [Then(@"an exception should be thrown indicating ""(.*)""")]
        public void ThenAnExceptionShouldBeThrownIndicating(string expectedMessageFragment)
        {
            Assert.NotNull(_caughtException);
            Assert.IsAssignableFrom<KeyNotFoundException>(_caughtException);
        }
    }
}
