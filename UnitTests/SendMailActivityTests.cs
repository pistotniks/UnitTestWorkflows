using System;
using System.Activities;
using System.Collections.Generic;
using System.Net.Mail;
using Activities;
using FluentAssertions;
using NUnit.Framework;

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
  }
}