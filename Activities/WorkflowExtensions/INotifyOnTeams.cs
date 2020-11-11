using System.Activities.Hosting;

namespace Activities.WorkflowExtensions
{
  public interface INotifyOnTeams : IWorkflowInstanceExtension
  {
    void Notify(string personName);
  }
}