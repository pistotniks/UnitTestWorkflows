using System.Activities.Hosting;

namespace Activities.WorkflowExtensions
{
  public interface ICanGetEmployeeFacts : IWorkflowInstanceExtension
  {
    bool IsEmployeeStillEmployed(string personName);
  }
}