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
      Dictionary<string, object> results = null;
      Exception ex = null;

      Dictionary<string, object> wfParams = new Dictionary<string, object>();
      wfParams.Add("To", "sepi@simcorp.com");
      wfParams.Add("From", "donotreply@example.org");
      wfParams.Add("Subject", "Unit testing");

      SmtpStatusCode instance = WorkflowInvoker.Invoke(new SendMailActivity(), wfParams);

      instance.Should().BeEquivalentTo(SmtpStatusCode.GeneralFailure);
    }
  }
}