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
      WorkflowApplicationProxy application = new WorkflowApplicationBuilder()
        .ForWorkflow(new ReportProcessing())
        .WithData("report", new ExpenseReportBuilder().DefaultData().Build())
        .Build();

      application.Run();

      var managerResponse = new ManagerResponse { Approved = true };
      application.ResumeBookmark("SubmitResponse", managerResponse);

      Retry.WaitUntil(TestContext.Progress).Execute(() => application.ActualOutputs != null);
      ManagerResponse actualResponse = (ManagerResponse)application.ActualOutputs["managerResponse"];

      // Assert
      application.VerifyAnError();
      
      actualResponse.Should().NotBeNull();
      actualResponse.Approved.Should().BeTrue();

      TestContext.Progress.WriteLine("Workflow completed and managers response to approve is - {0}", actualResponse.Approved.ToString());
    }

    // The test must fail, since we are throwing an exc in the workflow
    [Test]
    public void ShouldHandleException()
    {
      WorkflowApplicationProxy application = new WorkflowApplicationBuilder()
        .ForWorkflow(new ReportProcessing())
        .WithData("report", new ExpenseReportBuilder().DefaultData().Build())
        .Build();

      application.Run();

      // Simulating a null response, where the code will throw an exception
      ManagerResponse nullData = null;
      application.ResumeBookmark("SubmitResponse", nullData);

      Retry.WaitUntil(TestContext.Progress).Execute(() => application.Ex != null);

      application.VerifyAnError();
      Assert.Fail("Exception should have been thrown");
    }
  }
}