using Activities;
using Activities.WorkflowExtensions;
using EmployeeTodosApp;
using EmployeeTodosApp.ViewModel;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UnitTests.Builders;
using UnitTests.Core;
using ReportProcessing = EmployeeTodosApp.Workflows.ReportProcessing;

namespace UnitTests
{
  [TestFixture, Explicit]
  public class ReportProcessingWorkflowTests
  {
    // Happy path: The test should pass and expectations are set correctly
    [Test]
    public void ApproveAndResume()
    {
      var employeeRepositoryExtension = new Mock<ICanGetEmployeeFacts>();
      employeeRepositoryExtension.Setup(facts => facts.IsEmployeeStillEmployed(It.IsAny<string>())).Returns(true);
      WorkflowApplicationProxy application = new WorkflowApplicationBuilder()
        .ForWorkflow(new ReportProcessing())
        .WithData("report", new EmployeeTodoBuilder().DefaultData().Build())
        .WithCollaborator(employeeRepositoryExtension.Object)
        .Build();

      application.Run();

      var managerResponse = new ProductOwnerResponse { Approved = true };
      application.ResumeBookmark("SubmitResponse", managerResponse);

      Retry.WaitUntil(TestContext.Progress).Execute(() => application.ActualOutputs != null);
      ProductOwnerResponse actualResponse = (ProductOwnerResponse)application.ActualOutputs["managerResponse"];

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
      var employeeRepositoryExtension = new Mock<ICanGetEmployeeFacts>();
      employeeRepositoryExtension.Setup(facts => facts.IsEmployeeStillEmployed(It.IsAny<string>())).Returns(true);
      WorkflowApplicationProxy application = new WorkflowApplicationBuilder()
        .ForWorkflow(new ReportProcessing())
        .WithData("report", new EmployeeTodoBuilder().DefaultData().Build())
        .WithCollaborator(employeeRepositoryExtension.Object)
        .Build();

      application.Run();

      // Simulating a null response, where the code will throw an exception
      ProductOwnerResponse nullData = null;
      application.ResumeBookmark("SubmitResponse", nullData);

      Retry.WaitUntil(TestContext.Progress).Execute(() => application.Ex != null);

      application.VerifyAnError();
      Assert.Fail("Exception should have been thrown");
    }
  }
}