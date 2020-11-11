using Activities.Data;
using FluentAssertions;
using NUnit.Framework;
using UnitTests.Builders;
using UnitTests.Core;
using ReportProcessingWithDisapproval = EmployeeTodosApp.Workflows.ReportProcessingWithDisapproval;

namespace UnitTests
{
  public class WrongCodeToSimulateTestFailureWorkflowTests
  {
    // Testing a different workflow ReportProcessingWithDisapproval that has a simulated bug that resets the data to unexpected value
    // The test must fail (be red not green) with the proper log and fail on assertion that is logged
    [Test]
    public void ResumeButFailWithAWrongWorkflow()
    {
      // Assert
      WorkflowApplicationProxy application = new WorkflowApplicationBuilder()
        .ForWorkflow(new ReportProcessingWithDisapproval())
        .WithData("report", new EmployeeTodoBuilder().DefaultData().Build())
        .Build();

      application.Run();

      // Simulating the wrong code since the ReportProcessingWithDisapproval is not implemented correctly
      var managerResponse = new Manager { Approved = true };

      // Act
      application.ResumeBookmark("SubmitResponse", managerResponse);

      // Assert
      Retry.WaitUntil(TestContext.Progress).Execute(() => application.ActualOutputs != null);
      Manager actualResponse = (Manager)application.ActualOutputs["managerResponse"];

      application.VerifyAnError();

      actualResponse.Should().NotBeNull();
      actualResponse.Approved.Should().BeTrue();
    }
  }
}