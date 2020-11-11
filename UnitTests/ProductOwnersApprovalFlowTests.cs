﻿using System;
using Activities;
using Activities.WorkflowExtensions;
using EmployeeTodosApp;
using EmployeeTodosApp.ViewModel;
using EmployeeTodosApp.Workflows;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UnitTests.Builders;
using UnitTests.Core;

namespace UnitTests
{
  [TestFixture, Explicit]
  public class ProductOwnersApprovalFlowTests
  {
    // Happy path: The test should pass and expectations are set correctly
    [Test]
    public void ApproveAndResume()
    {
      // Arrange
      var notification = new Mock<INotifyOnTeams>();
      var employeeRepositoryExtension = new Mock<ICanGetEmployeeFacts>();
      // Simulating a big and complex and time consuming change in a database
      employeeRepositoryExtension.Setup(facts => facts.IsEmployeeStillEmployed(It.IsAny<string>())).Returns(true);
      
      WorkflowApplicationProxy application = new WorkflowApplicationBuilder()
        .ForWorkflow(new ProductOwnersApprovalFlow())
        .WithData("report", new EmployeeTodoBuilder().DefaultData().Build())
        .WithCollaborator(employeeRepositoryExtension.Object)
        .WithCollaborator(notification.Object)
        .Build();
      
      application.Run();

      var productOwnerResponse = new ProductOwnerResponse { Approved = true };
      // Act
      application.ResumeBookmark("SubmitResponse", productOwnerResponse);

      // Assert
      Retry.WaitUntil(TestContext.Progress).Execute(() => application.ActualOutputs != null);
      ProductOwnerResponse actualResponse = (ProductOwnerResponse)application.ActualOutputs["managerResponse"];

      application.VerifyAnError();
      
      actualResponse.Should().NotBeNull();
      actualResponse.Approved.Should().BeTrue();
      notification.Verify(notifier => notifier.Notify(It.IsAny<string>()), Times.Never);

      TestContext.Progress.WriteLine("Workflow completed and product owner response to approve is - {0}", actualResponse.Approved.ToString());
    }

    [Test]
    public void NotApproveAndShouldNotifyOnTeamsChannel_GivenEmployeeIsNotEmployedAnymore()
    {
      // Arrange
      var notification = new Mock<INotifyOnTeams>();
      var employeeRepositoryExtension = new Mock<ICanGetEmployeeFacts>();
      employeeRepositoryExtension.Setup(facts => facts.IsEmployeeStillEmployed(It.IsAny<string>())).Returns(false);

      WorkflowApplicationProxy application = new WorkflowApplicationBuilder()
        .ForWorkflow(new ProductOwnersApprovalFlow())
        .WithData("report", new EmployeeTodoBuilder().DefaultData().Build())
        .WithCollaborator(employeeRepositoryExtension.Object)
        .WithCollaborator(notification.Object)
        .Build();

      application.Run();

      var productOwnerResponse = new ProductOwnerResponse { Approved = true };
      // Act
      application.ResumeBookmark("SubmitResponse", productOwnerResponse);

      // Assert
      Retry.WaitUntil(TestContext.Progress).Execute(() => application.ActualOutputs != null);
      ProductOwnerResponse actualResponse = (ProductOwnerResponse)application.ActualOutputs["managerResponse"];

      application.VerifyAnError();

      actualResponse.Should().BeNull();
      notification.Verify(notifier => notifier.Notify("FooPersonName"), Times.Once);
    }

    // The test must fail, since we are throwing an exc in the workflow
    [Test]
    public void ShouldHandleException()
    {
      // Arrange
      var employeeRepositoryExtension = new Mock<ICanGetEmployeeFacts>();
      employeeRepositoryExtension.Setup(facts => facts.IsEmployeeStillEmployed(It.IsAny<string>())).Returns(true);
      
      WorkflowApplicationProxy application = new WorkflowApplicationBuilder()
        .ForWorkflow(new ProductOwnersApprovalFlow())
        .WithData("report", new EmployeeTodoBuilder().DefaultData().Build())
        .WithCollaborator(employeeRepositoryExtension.Object)
        .Build();

      application.Run();

      // Simulating a null response, where the code will throw an exception
      ProductOwnerResponse nullData = null;

      // Act
      application.ResumeBookmark("SubmitResponse", nullData);

      // Assert
      Retry.WaitUntil(TestContext.Progress).Execute(() => application.Ex != null);

      Assert.Throws<ApplicationException>(() => application.VerifyAnError());
    }
  }


}