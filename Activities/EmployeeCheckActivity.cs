using System;
using System.Activities;
using Activities.WorkflowExtensions;

namespace Activities
{
  public class EmployeeCheckActivity : CodeActivity<bool>
  {
    public InArgument<string> PersonName { get; set; }
    
    protected override bool Execute(CodeActivityContext context)
    {
      try
      {
        string personName = (string)PersonName.Get(context);
        ICanGetEmployeeFacts employeeFacts = context.GetExtension<ICanGetEmployeeFacts>();
        bool isEmployeeStillEmployed = employeeFacts.IsEmployeeStillEmployed(personName);
        return isEmployeeStillEmployed;
      }
      catch (Exception ex)
      {
        return false;
      }
    }
  }
}