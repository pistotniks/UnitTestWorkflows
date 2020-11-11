using System;
using System.Activities;
using Activities.WorkflowExtensions;

namespace Activities
{
  public class NotifyActivity : CodeActivity
  {
    public InArgument<string> PersonName { get; set; }

    protected override void Execute(CodeActivityContext context)
    {
      try
      {
        string personName = (string)PersonName.Get(context);
        INotifyOnTeams notifier = context.GetExtension<INotifyOnTeams>();
        notifier.Notify(personName);
      }
      catch (Exception ex)
      {
      }
    }
  }
}