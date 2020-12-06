using System;
using System.Activities;
using System.Collections.Generic;
using System.Net.Mail;
using Activities;
using EmployeeTodosApp.Workflows;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UnitTests.Builders;

namespace UnitTests
{
  public class SendMailActivityTests
  {
    [Test]
    public void SimpleActivity()
    {
      // Arrange
      Dictionary<string, object> wfParams = new Dictionary<string, object>();
      wfParams.Add("To", "sepi@simcorp.com");
      wfParams.Add("From", "donotreply@example.org");
      wfParams.Add("Subject", "Unit testing");

      // Act
      SmtpStatusCode instance = WorkflowInvoker.Invoke(new SendMailActivity(), wfParams);

      // Assert
      instance.Should().BeEquivalentTo(SmtpStatusCode.GeneralFailure);
    }

    [Test]
    public void SimpleActivity2()
    {
      // Arrange
      Dictionary<string, object> wfParams = new Dictionary<string, object>();
      wfParams.Add("To", "sepi@simcorp.com");
      wfParams.Add("From", "donotreply@example.org");
      wfParams.Add("Subject", "Unit testing");

      // Act
      // It works from here but not in designer!
      var smtpClientAdapter = new SmtpClientAdapter(new SmtpClient());
      SmtpStatusCode instance = WorkflowInvoker.Invoke(new SendMailActivity2(smtpClientAdapter), wfParams);

      // Assert
      instance.Should().BeEquivalentTo(SmtpStatusCode.GeneralFailure);
    }

    [Test]
    public void SimpleActivity3()
    {
      // Arrange
      var smtpClientAdapter = new Mock<ISmtpClient>();

      Dictionary<string, object> wfParams = new Dictionary<string, object>();
      wfParams.Add("Client", smtpClientAdapter.Object);
      wfParams.Add("To", "sepi@simcorp.com");
      wfParams.Add("From", "donotreply@example.org");
      wfParams.Add("Subject", "Unit testing");
      wfParams.Add("MailBody", "Drazen Petrovic");

      // Act
      // It works from here but not in designer!
      
      SmtpStatusCode instance = WorkflowInvoker.Invoke(new SendMailActivity3(), wfParams);

      // Assert
      instance.Should().BeEquivalentTo(SmtpStatusCode.GeneralFailure);

      // Trick from Heinz does not work, we need to improve further
      smtpClientAdapter.Verify(client => client.GetSmtpClient().Send("donotreply@example.org", "sepi@simcorp.com",
        "Unit testing", "Drazen Petrovic"), Times.Once);
    }

    [Test]
    public void SimpleActivity4()
    {
      // Arrange
      var smtpClientAdapter = new Mock<ISmtpClient2>();

      Dictionary<string, object> wfParams = new Dictionary<string, object>();
      wfParams.Add("Client", smtpClientAdapter.Object);
      wfParams.Add("To", "sepi@simcorp.com");
      wfParams.Add("From", "donotreply@example.org");
      wfParams.Add("Subject", "Unit testing");
      wfParams.Add("MailBody", "Drazen Petrovic");

      // Act
      // It works from here but not in designer!

      SmtpStatusCode instance = WorkflowInvoker.Invoke(new SendMailActivity4(), wfParams);

      // Assert
      instance.Should().BeEquivalentTo(SmtpStatusCode.Ok);

      // Trick from Heinz does not work, we need to improve further
      smtpClientAdapter.Verify(client => client.Send("donotreply@example.org", "sepi@simcorp.com",
        "Unit testing", "Drazen Petrovic"), Times.Once);
    }

    [Test]
    public void SimpleActivity6()
    {
      // Arrange
      var smtpClientAdapter = new Mock<ISmtpClient3>();

      WorkflowApplicationProxy application = new WorkflowApplicationBuilder()
        .ForWorkflow(new SendMailActivity6())
        .WithData("To", "sepi@simcorp.com")
        .WithData("From", "donotreply@example.org")
        .WithData("Subject", "Unit testing")
        .WithData("MailBody", "Drazen Petrovic")
        .WithCollaborator(smtpClientAdapter.Object)
        .Build();

      // Act
      application.Run();

      // Assert
      smtpClientAdapter.Verify(client => client.Send("donotreply@example.org", "sepi@simcorp.com",
        "Unit testing", "Drazen Petrovic"), Times.Once);
    }

    [Test]
    public void SimpleActivity7()
    {
      // Arrange
      var smtpClientAdapter = new Mock<ISendMailExtension>();

      WorkflowApplicationProxy application = new WorkflowApplicationBuilder()
        .ForWorkflow(new SendMailCodeActivity2())
        .WithData("To", "sepi@simcorp.com")
        .WithData("From", "donotreply@example.org")
        .WithData("Subject", "Unit testing")
        .WithData("MailBody", "Drazen Petrovic")
        .WithCollaborator(smtpClientAdapter.Object)
        .Build();

      // Act
      application.Run();

      // Assert
      var transformed = "Drazen Petrovic-transformed"; // I must know the intermediate calculation which is hidden as an internal impl detail
      smtpClientAdapter.Verify(client => client.Send("donotreply@example.org", "sepi@simcorp.com",
        "Unit testing", transformed), Times.Once);

      var expectedTransform = "Drazen Petrovic-super-transformed";
      application.ActualOutputs["TransformedMailBody"].Equals(expectedTransform);
    }

    [Test]
    public void SimpleActivity8()
    {
      // Arrange
      var smtpClientAdapter = new Mock<ISendMailExtension>();

      WorkflowApplicationProxy application = new WorkflowApplicationBuilder()
        .ForWorkflow(new SendEmailAndNextActivityPassingOutputOfFirst())
        .WithData("FromPerson", "donotreply@example.org")
        .WithCollaborator(smtpClientAdapter.Object)
        .Build();

      // Act
      application.Run();

      // Assert
      var transformed = "b-transformed"; // I must know the intermediate calculation which is hidden as an internal impl detail
      // smtpClientAdapter.Verify(client => client.Send("donotreply@example.org", "sepi@simcorp.com",
      //   "Unit testing", transformed), Times.Once);

      var expectedTransform = "b-super-transformed-super-transformed";
      var applicationActualOutput = application.ActualOutputs["WorkflowResult"];
      applicationActualOutput.Equals(expectedTransform);
    }
  }
}