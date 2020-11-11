using System.Activities.Hosting;
using System.Collections.Generic;

namespace Activities.WorkflowExtensions
{
  public class NotifyOnTeams : INotifyOnTeams
  {
    protected WorkflowInstanceProxy Instance { get; private set; }
    
    public IEnumerable<object> GetAdditionalExtensions()
    {
      return null;
    }

    public void SetInstance(WorkflowInstanceProxy instance)
    {
      Instance = instance;
    }

    public void Notify(string personName)
    {
    }
  }
}