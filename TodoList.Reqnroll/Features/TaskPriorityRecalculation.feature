Feature: Task Priority Recalculation
  As a user
  I want the task priority to be recalculated based on specific rules
  So that tasks are appropriately prioritized

Background:
  Given the current date is today for testing

Scenario Outline: Priority recalculation based on due date, keywords, and completion status
  Given a task with ID <TaskId>
  And the task description is "<Description>"
  And the task has a due date offset of "<DueDateOffsetDays>" days
  And the task is initially <IsCompleted>
  And the task has an initial priority of <InitialPriority>
  When the priority recalculation process is triggered for the task
  Then the new priority of the task should be <NewPriority>
  And the database update should be <Updated>

  Examples:
    | TaskId | Description                | DueDateOffsetDays | IsCompleted | InitialPriority | NewPriority | Updated |
    | 1      | "urgent task"              | -1                | false       | 0               | 2           | true    |
    | 2      | "critical task"            | -1                | false       | 2               | 2           | false   |
    | 3      | "urgent critical task"     | -1                | false       | 0               | 2           | true    |
    | 4      | "plain task"               | -1                | false       | 0               | 1           | true    |
    | 5      | "plain task due soon"      | 1                 | false       | 1               | 0           | true    |
    | 6      | "plain task far future"    | 3                 | false       | 0               | -1          | true    |
    | 7      | "task with no due date"    |                   | false       | 0               | -1          | true    |
    | 8      | "urgent completed task"    | -1                | true        | 2               | -1          | true    |
    | 9      | ""                         | -1                | false       | 0               | 1           | true    |
    | 12     | "boundary overdue task"    | -1                | false       | 0               | 1           | true    |
    | 13     | "boundary due soon task"   | 1                 | false       | 1               | 0           | true    |

Scenario: Recalculate priority for a task with null description leading to an exception
  Given a task with ID 10
  And the task description is null
  And the task has a due date offset of "-1" days
  And the task is initially false
  And the task has an initial priority of 0
  When the priority recalculation process is triggered for the task
  Then an exception should be thrown
  And the database update should be false

Scenario: Attempt to recalculate priority for an invalid Task ID
  Given an invalid Task ID 999
  When the priority recalculation process is triggered for the task with ID 999
  Then an exception should be thrown indicating "Task not found"
  And the database update should be false
