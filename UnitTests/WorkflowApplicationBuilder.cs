using System.Activities;
using System.Collections.Generic;

namespace UnitTests
{
  public class WorkflowApplicationBuilder
  {
    private Activity mActivity;
    private readonly Dictionary<string, object> mData = new Dictionary<string, object>();

    public WorkflowApplicationBuilder ForWorkflow(Activity activity)
    {
      mActivity = activity;
      return this;
    }

    public WorkflowApplicationProxy Build()
    {
      return new WorkflowApplicationProxy(mActivity, mData);
    }

    public WorkflowApplicationBuilder WithData(string key, object data)
    {
      mData.Add(key, data);
      return this;
    }
  }
}