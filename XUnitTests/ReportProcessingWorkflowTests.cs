using System;
using System.Activities;
using System.Collections.Generic;
using System.Threading;
using EmployeeExpensesApplication;
using ExpenseReporting;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XUnitTests
{
  public class ReportProcessingWorkflowTests
  {
    private ITestOutputHelper mTestOutputHelper;
    private Exception mEx = null;
    private bool mIsWorkflowCompleted;

    public ReportProcessingWorkflowTests(ITestOutputHelper testOutputHelper)
    {
      mTestOutputHelper = testOutputHelper;
    }

    // Happy path: The test should pass and expectations are set correctly
    [Fact]
    public void ApproveAndResume()
    {
      AutoResetEvent waitHandle = new AutoResetEvent(false);
      WorkflowApplication workflowApplication = new WorkflowApplication(
        new ReportProcessing(),
        new Dictionary<string, object>
        {
          {
            "report", new ExpenseReport
            {
              Employee = new Person(),
              StartDate = DateTime.Now,
              EndDate = DateTime.Now
            }
          }
        });

      workflowApplication.Completed += (wce) => OnCompleted(waitHandle, wce);
      workflowApplication.Idle += (wce) => OnIdle(waitHandle, wce);
      workflowApplication.OnUnhandledException += args => OnError(waitHandle, args);

      workflowApplication.Run();
      waitHandle.WaitOne();

      var managerResponse = new ManagerResponse {Approved = true};
      workflowApplication.ResumeBookmark("SubmitResponse", managerResponse);

      Retry.WaitUntil(mTestOutputHelper).Execute(() => mIsWorkflowCompleted);
      if (!mIsWorkflowCompleted)
      {
        // if the workflow is not completed after retrying, then the assertion failed many times and we fail the test
        throw new XunitException("Workflow failed due to assertion failing. See the logs for exception details.");
      }
    }

    // The test must fail, since we are throwing an exc in the workflow
    [Fact]
    public void ShouldHandleException()
    {
      AutoResetEvent waitHandle = new AutoResetEvent(false);
      WorkflowApplication wfApp = new WorkflowApplication(
        new ReportProcessing(),
        new Dictionary<string, object>
        {
          {
            "report", new ExpenseReport
            {
              Employee = new Person(),
              StartDate = DateTime.Now,
              EndDate = DateTime.Now
            }
          }
        });

      wfApp.Completed += (wce) => OnCompleted(waitHandle, wce);
      wfApp.Idle += (wce) => OnIdle(waitHandle, wce);
      wfApp.OnUnhandledException += args => OnError(waitHandle, args);

      wfApp.Run();
      waitHandle.WaitOne();

      // Simulating a null response, where the code will throw an exception
      ManagerResponse nullData = null;
      wfApp.ResumeBookmark("SubmitResponse", nullData);

      Retry.WaitUntil(mTestOutputHelper).Execute(() => mEx != null);

      if (mEx != null) throw mEx;
      throw new XunitException("Exception should have been thrown");
    }

    private void OnCompleted(AutoResetEvent waitHandle, WorkflowApplicationCompletedEventArgs wce)
    {
      var response = (ManagerResponse) wce.Outputs["managerResponse"];
      try
      {
        mTestOutputHelper.WriteLine("Workflow completed and managers response to approve is - {0}", response.Approved.ToString());

        try
        {
          response.Approved.Should().BeTrue();
        }
        catch (Exception e)
        {
          mTestOutputHelper.WriteLine(e.ToString());
          throw; // Very important to rethrow to be able to fail the test, or can we just set mIsWorkflowCompleted mIsWorkflowCompleted = false; and return;
        }
        
        mTestOutputHelper = null; // it already is disposed, but i am making it explicit, no need if a flag
        mIsWorkflowCompleted = true;
      }
      catch (Exception e)
      {
        // Do nothing, cause we do not have a logger anyway
      }

      waitHandle.Set();
    }

    private UnhandledExceptionAction OnError(AutoResetEvent waitHandle,
      WorkflowApplicationUnhandledExceptionEventArgs arg)
    {
      mTestOutputHelper.WriteLine(arg.UnhandledException.ToString());
      mEx = arg.UnhandledException;
      waitHandle.Set();
      return UnhandledExceptionAction.Abort;
    }

    private void OnIdle(AutoResetEvent waitHandle, WorkflowApplicationIdleEventArgs wce)
    {
      mTestOutputHelper.WriteLine("Workflow idle");
      waitHandle.Set();
    }
  }
}