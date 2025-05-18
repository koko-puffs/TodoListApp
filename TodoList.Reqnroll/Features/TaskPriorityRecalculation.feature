Feature: Task Priority Recalculation
  As a user
  I want the task priority to be recalculated based on specific rules
  So that tasks are appropriately prioritized

Background:
  Given the current date is today for testing

Scenario Outline: Priority recalculation based on due date, keywords, and completion status
  Given a task with ID <TaskId>
  And the task description is "<Description>"
  And the task due date is "<DueDateExpression>"
  And the task is initially <IsCompletedState>
  And the task has an initial priority of <InitialPriority>
  When the priority recalculation process is triggered for the task
  Then the new priority of the task should be <NewPriority>
  And the database update should be <UpdateStatus>

  Examples:
    | TaskId | Description                | DueDateExpression      | IsCompletedState | InitialPriority | NewPriority | UpdateStatus | Rule                                                |
    | 1      | "urgent task"              | "Today - 1 day"      | "not completed"  | 0               | 2           | "performed"  | TC1: Overdue, "urgent"                              |
    | 2      | "critical task"            | "Today - 1 day"      | "not completed"  | 2               | 2           | "skipped"    | TC2: Overdue, "critical", Prio same                 |
    | 3      | "urgent critical task"     | "Today - 1 day"      | "not completed"  | 0               | 2           | "performed"  | TC3: Overdue, "urgent" & "critical"                 |
    | 4      | "plain task"               | "Today - 1 day"      | "not completed"  | 0               | 1           | "performed"  | TC4: Overdue, No keywords                           |
    | 5      | "plain task due soon"      | "Today + 1 day"      | "not completed"  | 1               | 0           | "performed"  | TC5: Due Soon, No keywords                          |
    | 6      | "plain task far future"    | "Today + 3 days"     | "not completed"  | 0               | -1          | "performed"  | TC6: Far Future, No keywords                        |
    | 7      | "task with no due date"    | "null"               | "not completed"  | 0               | -1          | "performed"  | TC7: No DueDate, No keywords                        |
    | 8      | "urgent completed task"    | "Today - 1 day"      | "completed"      | 2               | -1          | "performed"  | TC8: Completed (was Overdue & Urgent)               |
    | 9      | ""                         | "Today - 1 day"      | "not completed"  | 0               | 1           | "performed"  | TC9: Empty Description, Overdue                     |
    | 12     | "boundary overdue task"    | "YesterdayEndOfDay"    | "not completed"  | 0               | 1           | "performed"  | TC12: Boundary: DueDate just Overdue                |
    | 13     | "boundary due soon task"   | "DueSoonUpperBoundary" | "not completed"  | 1               | 0           | "performed"  | TC13: Boundary: DueDate just Due Soon (upper)       |

Scenario: Recalculate priority for a task with null description leading to an exception
  Given a task with ID 10
  And the task description is null
  And the task due date is "Today - 1 day"
  And the task is initially "not completed"
  And the task has an initial priority of 0
  When the priority recalculation process is triggered for the task
  Then an exception should be thrown
  And the database update should be "skipped"

Scenario: Attempt to recalculate priority for an invalid Task ID
  Given an invalid Task ID 999
  When the priority recalculation process is triggered for the task with ID 999
  Then an exception should be thrown indicating "Task not found"
  And the database update should be "skipped"
