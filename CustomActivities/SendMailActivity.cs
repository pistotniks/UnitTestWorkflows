using System;
using System.Activities;
using System.Net.Mail;

namespace CustomActivities
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
}