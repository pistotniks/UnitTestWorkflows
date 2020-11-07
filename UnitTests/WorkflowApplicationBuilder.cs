using System.Activities;
using System.Activities.Hosting;
using System.Collections.Generic;

namespace UnitTests
{
  public class WorkflowApplicationBuilder
  {
    private Activity mActivity;
    private readonly Dictionary<string, object> mData = new Dictionary<string, object>();
    private readonly IList<IWorkflowInstanceExtension> mExtensions = new List<IWorkflowInstanceExtension>();

    public WorkflowApplicationBuilder ForWorkflow(Activity activity)
    {
      mActivity = activity;
      return this;
    }
    
    public WorkflowApplicationBuilder WithCollaborator(IWorkflowInstanceExtension collaborator)
    {
      mExtensions.Add(collaborator);
      return this;
    }

    public WorkflowApplicationBuilder WithData(string key, object data)
    {
      mData.Add(key, data);
      return this;
    }

    public WorkflowApplicationProxy Build()
    {
      return new WorkflowApplicationProxy(mActivity, mData, mExtensions);
    }
  }
}