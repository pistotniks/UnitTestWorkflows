using System.Activities.Hosting;

namespace Activities
{
  public interface ICanGetEmployeeFacts : IWorkflowInstanceExtension
  {
    bool IsEmployeeStillEmployed(string personName);
  }
}