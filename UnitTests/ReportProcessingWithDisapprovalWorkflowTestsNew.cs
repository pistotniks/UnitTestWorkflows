using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CustomActivities;
using EmployeeExpensesApplication;
using ExpenseReporting;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests
{
  public class ReportProcessingWithDisapprovalWorkflowTestsNew
  {
    // Testing a different workflow ReportProcessingWithDisapproval that has a simulated bug that resets the data to unexpected value
    // The test must fail (be red not green) with the proper log and fail on assertion that is logged
    [Test]
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

      Manager response = null;
      wfApp.Completed += (wce) =>
      {
        response = (Manager)wce.Outputs["managerResponse"];
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

      // Simulating the wrong code since the ReportProcessingWithDisapproval is not implemented correctly
      var managerResponse = new Manager {Approved = true};
      wfApp.ResumeBookmark("SubmitResponse", managerResponse);

      Retry.WaitUntil(TestContext.Progress).Execute(() => response != null);

      // Assert
      if (ex != null)
      {
        TestContext.Progress.WriteLine(ex);
        throw ex;
      }

      response.Should().NotBeNull();
      response.Approved.Should().BeTrue();
    }
  }
}