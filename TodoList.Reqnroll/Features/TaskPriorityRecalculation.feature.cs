﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by Reqnroll (https://www.reqnroll.net/).
//      Reqnroll Version:2.0.0.0
//      Reqnroll Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace TodoList.Reqnroll.Features
{
    using Reqnroll;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "2.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class TaskPriorityRecalculationFeature : object, Xunit.IClassFixture<TaskPriorityRecalculationFeature.FixtureData>, Xunit.IAsyncLifetime
    {
        
        private global::Reqnroll.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private static global::Reqnroll.FeatureInfo featureInfo = new global::Reqnroll.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Task Priority Recalculation", "  As a user\r\n  I want the task priority to be recalculated based on specific rule" +
                "s\r\n  So that tasks are appropriately prioritized", global::Reqnroll.ProgrammingLanguage.CSharp, featureTags);
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "TaskPriorityRecalculation.feature"
#line hidden
        
        public TaskPriorityRecalculationFeature(TaskPriorityRecalculationFeature.FixtureData fixtureData, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
        }
        
        public static async System.Threading.Tasks.Task FeatureSetupAsync()
        {
        }
        
        public static async System.Threading.Tasks.Task FeatureTearDownAsync()
        {
        }
        
        public async System.Threading.Tasks.Task TestInitializeAsync()
        {
            testRunner = global::Reqnroll.TestRunnerManager.GetTestRunnerForAssembly(featureHint: featureInfo);
            if (((testRunner.FeatureContext != null) 
                        && (testRunner.FeatureContext.FeatureInfo.Equals(featureInfo) == false)))
            {
                await testRunner.OnFeatureEndAsync();
            }
            if ((testRunner.FeatureContext == null))
            {
                await testRunner.OnFeatureStartAsync(featureInfo);
            }
        }
        
        public async System.Threading.Tasks.Task TestTearDownAsync()
        {
            await testRunner.OnScenarioEndAsync();
            global::Reqnroll.TestRunnerManager.ReleaseTestRunner(testRunner);
        }
        
        public void ScenarioInitialize(global::Reqnroll.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public async System.Threading.Tasks.Task ScenarioStartAsync()
        {
            await testRunner.OnScenarioStartAsync();
        }
        
        public async System.Threading.Tasks.Task ScenarioCleanupAsync()
        {
            await testRunner.CollectScenarioErrorsAsync();
        }
        
        public virtual async System.Threading.Tasks.Task FeatureBackgroundAsync()
        {
#line 6
#line hidden
#line 7
  await testRunner.GivenAsync("the current date is today for testing", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
        }
        
        async System.Threading.Tasks.Task Xunit.IAsyncLifetime.InitializeAsync()
        {
            await this.TestInitializeAsync();
        }
        
        async System.Threading.Tasks.Task Xunit.IAsyncLifetime.DisposeAsync()
        {
            await this.TestTearDownAsync();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="Priority recalculation based on due date, keywords, and completion status")]
        [Xunit.TraitAttribute("FeatureTitle", "Task Priority Recalculation")]
        [Xunit.TraitAttribute("Description", "Priority recalculation based on due date, keywords, and completion status")]
        [Xunit.InlineDataAttribute("1", "\"urgent task\"", "-1", "false", "0", "2", "true", new string[0])]
        [Xunit.InlineDataAttribute("2", "\"critical task\"", "-1", "false", "2", "2", "false", new string[0])]
        [Xunit.InlineDataAttribute("3", "\"urgent critical task\"", "-1", "false", "0", "2", "true", new string[0])]
        [Xunit.InlineDataAttribute("4", "\"plain task\"", "-1", "false", "0", "1", "true", new string[0])]
        [Xunit.InlineDataAttribute("5", "\"plain task due soon\"", "1", "false", "1", "0", "true", new string[0])]
        [Xunit.InlineDataAttribute("6", "\"plain task far future\"", "3", "false", "0", "-1", "true", new string[0])]
        [Xunit.InlineDataAttribute("7", "\"task with no due date\"", "", "false", "0", "-1", "true", new string[0])]
        [Xunit.InlineDataAttribute("8", "\"urgent completed task\"", "-1", "true", "2", "-1", "true", new string[0])]
        [Xunit.InlineDataAttribute("9", "\"\"", "-1", "false", "0", "1", "true", new string[0])]
        [Xunit.InlineDataAttribute("12", "\"boundary overdue task\"", "-1", "false", "0", "1", "true", new string[0])]
        [Xunit.InlineDataAttribute("13", "\"boundary due soon task\"", "1", "false", "1", "0", "true", new string[0])]
        public async System.Threading.Tasks.Task PriorityRecalculationBasedOnDueDateKeywordsAndCompletionStatus(string taskId, string description, string dueDateOffsetDays, string isCompleted, string initialPriority, string newPriority, string updated, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("TaskId", taskId);
            argumentsOfScenario.Add("Description", description);
            argumentsOfScenario.Add("DueDateOffsetDays", dueDateOffsetDays);
            argumentsOfScenario.Add("IsCompleted", isCompleted);
            argumentsOfScenario.Add("InitialPriority", initialPriority);
            argumentsOfScenario.Add("NewPriority", newPriority);
            argumentsOfScenario.Add("Updated", updated);
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Priority recalculation based on due date, keywords, and completion status", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 9
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 6
await this.FeatureBackgroundAsync();
#line hidden
#line 10
  await testRunner.GivenAsync(string.Format("a task with ID {0}", taskId), ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
#line 11
  await testRunner.AndAsync(string.Format("the task description is \"{0}\"", description), ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
#line 12
  await testRunner.AndAsync(string.Format("the task has a due date offset of \"{0}\" days", dueDateOffsetDays), ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
#line 13
  await testRunner.AndAsync(string.Format("the task is initially {0}", isCompleted), ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
#line 14
  await testRunner.AndAsync(string.Format("the task has an initial priority of {0}", initialPriority), ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
#line 15
  await testRunner.WhenAsync("the priority recalculation process is triggered for the task", ((string)(null)), ((global::Reqnroll.Table)(null)), "When ");
#line hidden
#line 16
  await testRunner.ThenAsync(string.Format("the new priority of the task should be {0}", newPriority), ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
#line 17
  await testRunner.AndAsync(string.Format("the database update should be {0}", updated), ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Recalculate priority for a task with null description leading to an exception")]
        [Xunit.TraitAttribute("FeatureTitle", "Task Priority Recalculation")]
        [Xunit.TraitAttribute("Description", "Recalculate priority for a task with null description leading to an exception")]
        public async System.Threading.Tasks.Task RecalculatePriorityForATaskWithNullDescriptionLeadingToAnException()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Recalculate priority for a task with null description leading to an exception", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 33
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 6
await this.FeatureBackgroundAsync();
#line hidden
#line 34
  await testRunner.GivenAsync("a task with ID 10", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
#line 35
  await testRunner.AndAsync("the task description is null", ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
#line 36
  await testRunner.AndAsync("the task has a due date offset of \"-1\" days", ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
#line 37
  await testRunner.AndAsync("the task is initially false", ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
#line 38
  await testRunner.AndAsync("the task has an initial priority of 0", ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
#line 39
  await testRunner.WhenAsync("the priority recalculation process is triggered for the task", ((string)(null)), ((global::Reqnroll.Table)(null)), "When ");
#line hidden
#line 40
  await testRunner.ThenAsync("an exception should be thrown", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
#line 41
  await testRunner.AndAsync("the database update should be false", ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Attempt to recalculate priority for an invalid Task ID")]
        [Xunit.TraitAttribute("FeatureTitle", "Task Priority Recalculation")]
        [Xunit.TraitAttribute("Description", "Attempt to recalculate priority for an invalid Task ID")]
        public async System.Threading.Tasks.Task AttemptToRecalculatePriorityForAnInvalidTaskID()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Attempt to recalculate priority for an invalid Task ID", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 43
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 6
await this.FeatureBackgroundAsync();
#line hidden
#line 44
  await testRunner.GivenAsync("an invalid Task ID 999", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
#line 45
  await testRunner.WhenAsync("the priority recalculation process is triggered for the task with ID 999", ((string)(null)), ((global::Reqnroll.Table)(null)), "When ");
#line hidden
#line 46
  await testRunner.ThenAsync("an exception should be thrown indicating \"Task not found\"", ((string)(null)), ((global::Reqnroll.Table)(null)), "Then ");
#line hidden
#line 47
  await testRunner.AndAsync("the database update should be false", ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "2.0.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : object, Xunit.IAsyncLifetime
        {
            
            async System.Threading.Tasks.Task Xunit.IAsyncLifetime.InitializeAsync()
            {
                await TaskPriorityRecalculationFeature.FeatureSetupAsync();
            }
            
            async System.Threading.Tasks.Task Xunit.IAsyncLifetime.DisposeAsync()
            {
                await TaskPriorityRecalculationFeature.FeatureTearDownAsync();
            }
        }
    }
}
#pragma warning restore
#endregion
