using System;
using System.Activities;
using System.Collections.Generic;
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
    // Happy path: The test should pass and expectations are set correctly
    [Test]
    public void ApproveAndResume()
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

      ManagerResponse actualResponse = null;
      wfApp.Completed += (wce) =>
      {
        actualResponse = (ManagerResponse)wce.Outputs["managerResponse"];
        waitHandle.Set();
      };
      wfApp.Idle += (wce) =>
      {
        waitHandle.Set();
      };
      Exception ex = null;
      wfApp.OnUnhandledException += arg =>
      {
        ex = arg.UnhandledException;
        waitHandle.Set();
        return UnhandledExceptionAction.Abort;
      };

      wfApp.Run();
      waitHandle.WaitOne();

      var managerResponse = new ManagerResponse {Approved = true};
      wfApp.ResumeBookmark("SubmitResponse", managerResponse);

      Retry.WaitUntil(TestContext.Progress).Execute(() => actualResponse != null);

      // Assert
      if (ex != null)
      {
        TestContext.Progress.WriteLine(ex);
        throw ex;
      }

      actualResponse.Should().NotBeNull();
      TestContext.Progress.WriteLine("Workflow completed and managers response to approve is - {0}", actualResponse.Approved.ToString());
      actualResponse.Approved.Should().BeTrue();
    }

    // The test must fail, since we are throwing an exc in the workflow
    [Test]
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

      wfApp.Idle += (wce) =>
      {
        waitHandle.Set();
      };
      Exception ex = null;
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

      Retry.WaitUntil(TestContext.Progress).Execute(() => ex != null);

      if (ex != null)
      {
        TestContext.Progress.WriteLine(ex);
        throw ex;
      }
      Assert.Fail("Exception should have been thrown");
    }
  }
}