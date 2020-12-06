using Activities;
using Activities.WorkflowExtensions;
using EmployeeTodosApp.ViewModel;
using EmployeeTodosApp.Workflows;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UnitTests.Builders;

namespace UnitTests
{
  public class ProductOwnersApprovalFlowThatDoesNotSwallowActivitiesWithNonDefConstrTests
  {
    [Test]
    public void Do()
    {
      // Arrange
      var smtpClientAdapter = new Mock<ISmtpClient2>();

      var flow = new ProductOwnersApprovalFlowThatDoesNotSwallowActivitiesWithNonDefConstr();
      WorkflowApplicationProxy application = new WorkflowApplicationBuilder()
        .ForWorkflow(flow)
        .WithData("SmtpClientToAbleToMock", smtpClientAdapter.Object)
        .WithData("FromPerson", "me@gmail.com")
        .Build();

      application.Run();

      // Assert
      smtpClientAdapter.Verify(client => client.Send("me@gmail.com", "test@gmail.com",
        "SomeSubject", "SomeBody"), Times.Once);
    }

  }
}