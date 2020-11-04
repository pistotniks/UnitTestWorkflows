using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using EmployeeExpensesApplication;
using ExpenseReporting;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests
{
  [TestFixture, Explicit]
  public class ReportProcessingWorkflowTests
  {
    private Exception mEx = null;
    private TextWriter mTextWriter;
    private bool mIsWorkflowCompleted;

    [SetUp]
    public void FixtureSetUp()
    {
    }

    // Happy path: The test should pass and expectations are set correctly
    [Test]
    public void ApproveAndResume()
    {
      mTextWriter = TestContext.Progress;
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

      var managerResponse = new ManagerResponse {Approved = true};
      wfApp.ResumeBookmark("SubmitResponse", managerResponse);

      Retry.WaitUntil(mTextWriter).Execute(() => mIsWorkflowCompleted);
      if (!mIsWorkflowCompleted)
      {
        // if the workflow is not completed after retrying, then the assertion failed many times and we fail the test
        Assert.Fail("Workflow failed due to assertion failing");
      }
    }


    // The test must fail, since we are throwing an exc in the workflow
    [Test]
    public void ShouldHandleException()
    {
      mTextWriter = TestContext.Progress;
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

      Retry.WaitUntil(mTextWriter).Execute(() => mEx != null);

      if (mEx != null) throw mEx;
      Assert.Fail("Exception should have been thrown");
    }

    private void OnCompleted(AutoResetEvent waitHandle, WorkflowApplicationCompletedEventArgs wce)
    {
      var response = (ManagerResponse) wce.Outputs["managerResponse"];

      try
      {
        mTextWriter.WriteLine("Workflow completed and managers response to approve is - {0}", response.Approved.ToString());

        try
        {
          response.Approved.Should().BeTrue();
        }
        catch (Exception e)
        {
          mTextWriter.WriteLine(e.ToString());
          throw; // Very important to rethrow to be able to fail the test, or can we just set mIsWorkflowCompleted mIsWorkflowCompleted = false; and return;
        }

        mTextWriter = null; // it already is disposed, but i am making it explicit, no need if a flag
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
      mTextWriter.WriteLine("Ex.:" + arg.UnhandledException);
      mEx = arg.UnhandledException;
      waitHandle.Set();
      return UnhandledExceptionAction.Abort;
    }
    
    private void OnIdle(AutoResetEvent waitHandle, WorkflowApplicationIdleEventArgs wce)
    {
      mTextWriter.WriteLine("Workflow idle");
      waitHandle.Set();
    }
  }
}