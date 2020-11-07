using System.Activities.Hosting;

namespace CustomActivities
{
  public interface ICanGetEmployeeFacts : IWorkflowInstanceExtension
  {
    bool IsEmployeeStillEmployed(string personName);
  }
}