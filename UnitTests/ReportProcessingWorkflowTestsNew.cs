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
  public class ReportProcessingWorkflowTestsNew
  {
    // Happy path: The test should pass and expectations are set correctly
    [Test]
    public void ApproveAndResume()
    {
      bool isWorkflowCompleted = false;
      Exception ex = null;
      TextWriter textWriter = TestContext.Progress;
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

      ManagerResponse response = null;
      wfApp.Completed += (wce) =>
      {
        response = (ManagerResponse)wce.Outputs["managerResponse"];
        isWorkflowCompleted = true;
        waitHandle.Set();
      };
      wfApp.Idle += (wce) =>
      {
        waitHandle.Set();
      };
      wfApp.OnUnhandledException += arg =>
      {
        textWriter.WriteLine("Ex.:" + arg.UnhandledException);
        ex = arg.UnhandledException;
        waitHandle.Set();
        return UnhandledExceptionAction.Abort;
      };

      wfApp.Run();
      waitHandle.WaitOne();

      var managerResponse = new ManagerResponse {Approved = true};
      wfApp.ResumeBookmark("SubmitResponse", managerResponse);

      Retry.WaitUntil(textWriter).Execute(() => isWorkflowCompleted);
      if (!isWorkflowCompleted)
      {
        // if the workflow is not completed after retrying, then the assertion failed many times and we fail the test
        Assert.Fail("Workflow failed due to assertion failing. See the logs for exception details.");
      }

      // Assert
      textWriter.WriteLine("Workflow completed and managers response to approve is - {0}", response.Approved.ToString());
      response.Approved.Should().BeTrue();
    }


    // The test must fail, since we are throwing an exc in the workflow
    [Test]
    public void ShouldHandleException()
    {
      Exception ex = null;
      TextWriter textWriter = TestContext.Progress;
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

      wfApp.Idle += (wce) =>
      {
        waitHandle.Set();
      };
      wfApp.OnUnhandledException += args =>
      {
        ex = args.UnhandledException;
        waitHandle.Set();
        return UnhandledExceptionAction.Abort;
      };

      wfApp.Run();
      waitHandle.WaitOne();

      // Simulating a null response, where the code will throw an exception
      ManagerResponse nullData = null;
      wfApp.ResumeBookmark("SubmitResponse", nullData);

      Retry.WaitUntil(textWriter).Execute(() => ex != null);

      if (ex != null)
      {
        textWriter.WriteLine(ex);
        throw ex;
      }
      Assert.Fail("Exception should have been thrown");
    }
  }
}