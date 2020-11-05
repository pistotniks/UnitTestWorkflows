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
      bool isWorkflowCompleted = false;
      Exception ex = null;
      TextWriter textWriter = TestContext.Progress;
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
        isWorkflowCompleted = true;
        waitHandle.Set();
      };
      wfApp.Idle += (wce) =>
      {
        waitHandle.Set();
      };
      wfApp.OnUnhandledException += arg =>
      {
        ex = arg.UnhandledException;
        waitHandle.Set();
        return UnhandledExceptionAction.Abort;
      };

      wfApp.Run();
      waitHandle.WaitOne();

      var managerResponse = new Manager();
      managerResponse.Approved = true; // Simulating the wrong code since the ReportProcessingWithDisapproval is not implemented correctly
      wfApp.ResumeBookmark("SubmitResponse", managerResponse);

      Retry.WaitUntil(textWriter).Execute(() => isWorkflowCompleted);
      if (!isWorkflowCompleted)
      {
        // if the workflow is not completed after retrying, then the assertion failed many times and we fail the test
        Assert.Fail("Workflow failed due to assertion failing. See the logs for exception details.");
      }

      // Assert
      if (ex != null)
      {
        textWriter.WriteLine(ex);
        throw ex;
      }
      response.Approved.Should().BeTrue();
    }
  }
}