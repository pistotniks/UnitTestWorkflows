﻿using CustomActivities;
using EmployeeExpensesApplication;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests
{
  public class ReportProcessingWithDisapprovalWorkflowTests
  {
    // Testing a different workflow ReportProcessingWithDisapproval that has a simulated bug that resets the data to unexpected value
    // The test must fail (be red not green) with the proper log and fail on assertion that is logged
    [Test]
    public void ResumeButFailWithAWrongWorkflow()
    {
      WorkflowApplicationProxy application = new WorkflowApplicationBuilder()
        .ForWorkflow(new ReportProcessingWithDisapproval())
        .WithData("report", new ExpenseReportBuilder().DefaultData().Build())
        .Build();

      application.Run();

      // Simulating the wrong code since the ReportProcessingWithDisapproval is not implemented correctly
      var managerResponse = new Manager { Approved = true };
      application.ResumeBookmark("SubmitResponse", managerResponse);

      Retry.WaitUntil(TestContext.Progress).Execute(() => application.ActualOutputs != null);
      Manager actualResponse = (Manager)application.ActualOutputs["managerResponse"];

      // Assert
      application.VerifyAnError();

      actualResponse.Should().NotBeNull();
      actualResponse.Approved.Should().BeTrue();
    }
  }
}