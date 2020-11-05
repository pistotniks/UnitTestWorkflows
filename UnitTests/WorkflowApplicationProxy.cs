using System;
using System.Activities;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace UnitTests
{
  public class WorkflowApplicationProxy
  {
    private readonly WorkflowApplication mApplication;
    private Exception mEx;
    private AutoResetEvent mWaitHandle;
    private IDictionary<string, object> mActualOutputs;

    public WorkflowApplicationProxy(Activity activity, Dictionary<string, object> data)
    {
      mWaitHandle = new AutoResetEvent(false);

      mApplication = new WorkflowApplication(activity, data);
      mApplication.Completed += (wce) =>
      {
        mActualOutputs = wce.Outputs;
        mWaitHandle.Set();
      };
      mApplication.Idle += (wce) =>
      {
        mWaitHandle.Set();
      };
      mApplication.OnUnhandledException += arg =>
      {
        mEx = arg.UnhandledException;
        mWaitHandle.Set();
        return UnhandledExceptionAction.Abort;
      };
    }

    public void Run()
    {
      mApplication.Run();
      mWaitHandle.WaitOne();
    }

    public BookmarkResumptionResult ResumeBookmark(
      string bookmarkName,
      object value)
    {
      return mApplication.ResumeBookmark(bookmarkName, value);
    }

    public Exception Ex => mEx;

    public IDictionary<string, object> ActualOutputs => mActualOutputs;

    public void VerifyAnError()
    {
      if (Ex != null)
      {
        TestContext.Progress.WriteLine(Ex);
        throw Ex;
      }
    }
  }
}