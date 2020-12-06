using System;
using System.Activities;
using System.Activities.Hosting;
using System.Collections.Generic;
using System.Net.Mail;

namespace Activities
{
  public class SendMailActivity : CodeActivity<SmtpStatusCode>
  {
    public InArgument<string> To { get; set; }
    public InArgument<string> From { get; set; }
    public InArgument<string> Subject { get; set; }
    public InArgument<string> MailBody { get; set; }

    protected override SmtpStatusCode Execute(CodeActivityContext context)
    {
      try
      {
        SmtpClient client = new SmtpClient();
        string s = From.Get(context);
        var body = MailBody.Get(context);
        client.Send(s,
          To.Get(context),
          Subject.Get(context),
          body);

        // A possible False positive (You are trusting the writer of send method; Send might swallow all of exception in case of dumm writer or for some other reason); => We need to assert against whether or not Send was called
        return SmtpStatusCode.Ok;
      }
      catch (SmtpException ex)
      {
        return ex.StatusCode;
      }
      catch (Exception ex)
      {
        return SmtpStatusCode.GeneralFailure;
      }
    }
  }

  // An invalid try to improve the activity and make it testable, to be able to assert against whether or not send was really called
  public class SendMailActivity2 : CodeActivity<SmtpStatusCode>
  {
    private ISmtpClient mClient;
    public InArgument<string> To { get; set; }
    public InArgument<string> From { get; set; }
    public InArgument<string> Subject { get; set; }
    public InArgument<string> MailBody { get; set; }

    // Needed by designer
    public SendMailActivity2()
    {
    }

    // It might work in test of this activity, but you cannot add it to big workflows...so this is not valid!
    public SendMailActivity2(ISmtpClient client)
    {
      mClient = client;
    }

    protected override SmtpStatusCode Execute(CodeActivityContext context)
    {
      try
      {
        SmtpClient c = mClient.GetSmtpClient();
        string s = From.Get(context);
        var body = MailBody.Get(context);
        c.Send(s,
          To.Get(context),
          Subject.Get(context),
          body);

        return SmtpStatusCode.Ok;
      }
      catch (SmtpException ex)
      {
        return ex.StatusCode;
      }
      catch (Exception ex)
      {
        return SmtpStatusCode.GeneralFailure;
      }
    }
  }

  public class SmtpClientAdapter : ISmtpClient
  {
    private readonly SmtpClient mSmtpClient;

    public SmtpClientAdapter(SmtpClient smtpClient)
    {
      mSmtpClient = smtpClient;
    }

    // A trick to avoid delegate the calls
    public SmtpClient GetSmtpClient()
    {
      return mSmtpClient;
    }
  }

  public interface ISmtpClient
  {
    SmtpClient GetSmtpClient();
  }

  public class SendMailActivity3 : CodeActivity<SmtpStatusCode>
  {
    public InArgument<ISmtpClient> Client { get; set; }
    public InArgument<string> To { get; set; }
    public InArgument<string> From { get; set; }
    public InArgument<string> Subject { get; set; }
    public InArgument<string> MailBody { get; set; }

    public SendMailActivity3()
    {
    }

    protected override SmtpStatusCode Execute(CodeActivityContext context)
    {
      try
      {
        ISmtpClient smtpClient = Client.Get(context);
        SmtpClient c = smtpClient.GetSmtpClient();
        string s = From.Get(context);
        var body = MailBody.Get(context);
        c.Send(s,
          To.Get(context),
          Subject.Get(context),
          body);

        return SmtpStatusCode.Ok;
      }
      catch (SmtpException ex)
      {
        return ex.StatusCode;
      }
      catch (Exception ex)
      {
        return SmtpStatusCode.GeneralFailure;
      }
    }
  }

  public class SmtpClientAdapter2 : ISmtpClient2
  {
    private readonly SmtpClient mSmtpClient;

    public SmtpClientAdapter2(SmtpClient smtpClient)
    {
      mSmtpClient = smtpClient;
    }

    // A trick to avoid delegate the calls
    public void Send(string from, string recipients, string subject, string body)
    {
      mSmtpClient.Send(from, recipients, subject, body);
    }
  }

  public interface ISmtpClient2
  {
    void Send(string from, string recipients, string subject, string body);
  }

  public class SendMailActivity4 : CodeActivity<SmtpStatusCode>
  {
    public InArgument<ISmtpClient2> Client { get; set; }
    public InArgument<string> To { get; set; }
    public InArgument<string> From { get; set; }
    public InArgument<string> Subject { get; set; }
    public InArgument<string> MailBody { get; set; }

    protected override SmtpStatusCode Execute(CodeActivityContext context)
    {
      try
      {
        ISmtpClient2 c = Client.Get(context);
        string s = From.Get(context);
        var body = MailBody.Get(context);
        c.Send(s,
          To.Get(context),
          Subject.Get(context),
          body);

        return SmtpStatusCode.Ok;
      }
      catch (SmtpException ex)
      {
        return ex.StatusCode;
      }
      catch (Exception ex)
      {
        return SmtpStatusCode.GeneralFailure;
      }
    }
    // https://code-coverage.net/workflow-foundation-tutorial-part-2-the-basics/
    // So my activity IS testable. It has a property dep. injection (not const injection). The problem is the way it is now, you will have to create a variable or argument in the designer every time you use this activity to be able to inject this.
    // The problem is that maybe I want to have my dependency injection even more fancy. I want to do it with magic.
    // Like include MEF, Service Locator pattern? Well no, it turns out that MS has its own Service Locator pattern called Extensions.
    // It could be named Services (like in .net Core API apps ConfigureServices(IServiceCollection services)), Collaborators or whatever.
  }
  // I would like this:
  public class SendMailActivity5 : CodeActivity<SmtpStatusCode>
  {
    //public InArgument<ISmtpClient2> Client { get; set; }
    public InArgument<string> To { get; set; }
    public InArgument<string> From { get; set; }
    public InArgument<string> Subject { get; set; }
    public InArgument<string> MailBody { get; set; }

    protected override SmtpStatusCode Execute(CodeActivityContext context)
    {
      try
      {
        // Now I do not need to declare a variable in designer plus I loose a line of code in here: The drawback is: I get MAGIC.
        ISmtpClient3 c = context.GetExtension<ISmtpClient3>();
        string s = From.Get(context);
        var body = MailBody.Get(context);
        c.Send(s,
          To.Get(context),
          Subject.Get(context),
          body);

        return SmtpStatusCode.Ok;
      }
      catch (SmtpException ex)
      {
        return ex.StatusCode;
      }
      catch (Exception ex)
      {
        return SmtpStatusCode.GeneralFailure;
      }
    }
  }

  public class SmtpClientAdapter3 : ISmtpClient3
  {
    private readonly SmtpClient mSmtpClient;

    public SmtpClientAdapter3(SmtpClient smtpClient)
    {
      mSmtpClient = smtpClient;
    }

    // A trick to avoid delegate the calls
    public void Send(string from, string recipients, string subject, string body)
    {
      mSmtpClient.Send(from, recipients, subject, body);
    }

    public IEnumerable<object> GetAdditionalExtensions()
    {
      throw new NotImplementedException();
    }

    public void SetInstance(WorkflowInstanceProxy instance)
    {
      throw new NotImplementedException();
    }
  }

  public interface ISmtpClient3 : IWorkflowInstanceExtension
  {
    void Send(string from, string recipients, string subject, string body);
  }

  public class SendMailActivity6 : CodeActivity<SmtpStatusCode>
  {
    public InArgument<string> To { get; set; }
    public InArgument<string> From { get; set; }
    public InArgument<string> Subject { get; set; }
    public InArgument<string> MailBody { get; set; }

    protected override SmtpStatusCode Execute(CodeActivityContext context)
    {
      try
      {
        // Now you think: Well...why don't I move everything into extension, why do I have this logic here; Anwser No: This is a perfect activity
        ISmtpClient3 c = context.GetExtension<ISmtpClient3>();
        string @from = From.Get(context);
        var body = MailBody.Get(context);
        var recipients = To.Get(context);
        var subject = Subject.Get(context);
        c.Send(
          @from,
          recipients,
          subject,
          body);

        return SmtpStatusCode.Ok;
      }
      catch (SmtpException ex)
      {
        return ex.StatusCode;
      }
      catch (Exception ex)
      {
        return SmtpStatusCode.GeneralFailure;
      }
    }
  }

  // I can do this:
  public class SendMailCodeActivity : CodeActivity<SmtpStatusCode>
  {
    public InArgument<string> To { get; set; }
    public InArgument<string> From { get; set; }
    public InArgument<string> Subject { get; set; }
    public InArgument<string> MailBody { get; set; }

    protected override SmtpStatusCode Execute(CodeActivityContext context)
    {
      try
      {
        ISendMailExtension c = context.GetExtension<ISendMailExtension>();
        string @from = From.Get(context);
        var body = MailBody.Get(context);
        var recipients = To.Get(context);
        var subject = Subject.Get(context);
        c.Send(
          @from,
          recipients,
          subject,
          body);

        return SmtpStatusCode.Ok;
      }
      catch (SmtpException ex)
      {
        return ex.StatusCode;
      }
      catch (Exception ex)
      {
        return SmtpStatusCode.GeneralFailure;
      }
    }
  }

  public interface ISendMailExtension : IWorkflowInstanceExtension
  {
    void Send(string from, string recipients, string subject, string body);
  }

  // Or even better; just 
  public interface ISendMail : IWorkflowInstanceExtension
  {
    void Send(string from, string recipients, string subject, string body);
  }

  public class SendMailCodeActivity2 : CodeActivity<SmtpStatusCode>
  {
    public InArgument<string> To { get; set; }
    public InArgument<string> From { get; set; }
    public InArgument<string> Subject { get; set; }
    public InArgument<string> MailBody { get; set; }

    public InOutArgument<string> TransformedMailBody { get; set; }

    protected override SmtpStatusCode Execute(CodeActivityContext context)
    {
      try
      {
        // An activiy becomes a holder for extension that just delegate a call; or many of them
        ISendMailExtension c = context.GetExtension<ISendMailExtension>();
        string @from = From.Get(context);
        var recipients = To.Get(context);
        var subject = Subject.Get(context);
        // Can I do this? OR should I do this in extension OR should I do this in workflow designer?
        if (string.IsNullOrEmpty(subject))
        {
          return SmtpStatusCode.GeneralFailure;
        }

        var transformBodyPassedIn = TransformedMailBody.Get(context); // will always be null
        // OR this calculation before passing it into the extension?
        var body = Transform(MailBody.Get(context)); //NO!; you should delegate the transform to extension or put it into another activity and drop it into designer, otherwise you cannot stub it without saying Any
        // If I do this if here: My activity needs to have a unit test to have a test whether or not someone removed this "complex" condition/branch/decision
        // If I do this in ISendMailExtension then we need to have a test for the impl of ISendMailExtension
        // If I have an if in workflow big workflow, defined in designer; then i need to have a test for workflow designer

        // What do I get with if check declared in designer? And not in extension for example.
        // The customer can see it and evaluate is this ok and we can have a conversation. That is the purpose of a declarative respresantion of functinality in designer.

        // But what if we do not have a customer? Which in case of our Cristians workflow, we do not. He has changed a workflow.
        // Then it is better if we move this either to extension or activity and unit test it. In this case it would not matter where as long as you think of single responsiblity, as both are testable.

        // But which thing is better if we do not have customer, but we have a current situstaion (a reality, a spageti code) where the customer is not changing the
        // workflow and have the responsibility to make it work, but customer evaluates us based on what kind of workflow we give them (it has to work and so it must be
        // tested) and that workflow currenlty unfortunatelly HAS ifs and loops and so on. 
        // In this case we MUST make workflows designer testable and test them.
        // Which would mean we need to create an environment and conventions where we can predict what happens. We must not have any logic/ifs/loops in our activities.
        // All the logic needs to go to extensions. Activities should be a synonim for extension. It always just delegate a call.
        // WHY
        // Cause if you have if you have an if in desginer workflow or here; You need also to have a test coverage of a least 2 tests for it (which is tedious cause of the amount of the dependcies you carry along; it is easier in extension).
        
        // The calculated body cannot be ok, even if we test just this activity. Since we cannot setup the call to Send. We need to know how to calculate transform, which is a problem and we cannot.
        c.Send(
          @from,
          recipients,
          subject,
          body);

        // What about this; calculation after a call to extension? YES ONLY IF, we set the output prm (so we can test it with an activity) and we do not use/reuse the activity designer.
        // YES if we do not have to verify or set it as input prm to stubbed method call in the next activity: No; it just looks like right cause you are setting the output arg, but if you reuse this activity in next activity and that value goes into the next method call, you are not able neither to verify neither to stub.
        // Same as if you call send again with new body like below
        body = SuperTransform(MailBody.Get(context)); // Or new FooBody()

        // Not possible to verify or stub unless knowing the algorithm for SuperTransform, since we need a different body when setting the mock or verifying it
        c.Send(
          @from,
          recipients,
          subject,
          body);

        TransformedMailBody.Set(context, body);

        return SmtpStatusCode.Ok;
      }
      catch (SmtpException ex)
      {
        return ex.StatusCode;
      }
      catch (Exception ex)
      {
        return SmtpStatusCode.GeneralFailure;
      }
    }

    // with side eff or no side effect, it does not matter
    private string Transform(string body)
    {
      return body + "-transformed";
    }

    private string SuperTransform(string body)
    {
      return body + "-super-transformed";
    }
  }
}