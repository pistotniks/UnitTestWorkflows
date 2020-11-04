using System;
using System.Activities;
using System.Collections.Generic;
using System.Threading;
using CustomActivities;
using EmployeeExpensesApplication;
using ExpenseReporting;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XUnitTests
{
  public class ReportProcessingWithDisapprovalWorkflowTests
  {
    private ITestOutputHelper mTestOutputHelper;
    private Exception mEx = null;
    private bool mIsWorkflowCompleted;

    public ReportProcessingWithDisapprovalWorkflowTests(ITestOutputHelper testOutputHelper)
    {
      mTestOutputHelper = testOutputHelper;
    }

    // Testing a different workflow ReportProcessingWithDisapproval that has a simulated bug that resets the data to unexpected value
    // The test must fail (be red not green) with the proper log and fail on assertion that is logged
    [Fact]
    public void ResumeButFailWithAWrongWorkflow()
    {
      AutoResetEvent waitHandle = new AutoResetEvent(false);
      WorkflowApplication wfApp = new WorkflowApplication(
        new ReportProcessingWithDisapproval(),
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

      wfApp.Completed += (wce) => OnCompletedOfDisapproval(waitHandle, wce);
      wfApp.Idle += (wce) => OnIdle(waitHandle, wce);
      wfApp.OnUnhandledException += args => OnError(waitHandle, args);

      wfApp.Run();
      waitHandle.WaitOne();

      var managerResponse = new Manager();
      managerResponse.Approved = true; // Simulating the wrong code since the ReportProcessingWithDisapproval is not implemented correctly
      wfApp.ResumeBookmark("SubmitResponse", managerResponse);

      Retry.WaitUntil(mTestOutputHelper).Execute(() => mIsWorkflowCompleted);
      if (!mIsWorkflowCompleted)
      {
        // if the workflow is not completed after retrying, then the assertion failed many times and we fail the test
        throw new XunitException("Workflow failed due to assertion failing");
      }
    }

    private void OnCompletedOfDisapproval(AutoResetEvent waitHandle, WorkflowApplicationCompletedEventArgs wce)
    {
      var response = (Manager)wce.Outputs["managerResponse"];
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