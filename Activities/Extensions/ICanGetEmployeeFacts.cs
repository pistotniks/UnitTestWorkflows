using System.Activities.Hosting;

namespace Activities.Extensions
{
  public interface ICanGetEmployeeFacts : IWorkflowInstanceExtension
  {
    bool IsEmployeeStillEmployed(string personName);
  }
}