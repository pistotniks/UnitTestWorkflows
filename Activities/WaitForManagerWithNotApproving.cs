using System;
using System.Activities;
using Activities.Data;

namespace Activities
{
  public sealed class WaitForManagerWithNotApproving<T> : NativeActivity<T>
  {
    [RequiredArgument] public InArgument<string> BookmarkName { get; set; }

    protected override bool CanInduceIdle
    {
      get { return true; }
    }

    protected override void Execute(NativeActivityContext context)
    {
      context.CreateBookmark(
        BookmarkName.Get(context),
        new BookmarkCallback(DataArrived), BookmarkOptions.None);
    }

    public void DataArrived(NativeActivityContext context,
      Bookmark bk, object data)
    {
      if (data == null)
      {
        throw new ApplicationException("Foo");
      }

      Manager manager = data as Manager;
      // Simulation of the wrong code (setting the approved to false), so the test must fail
      manager.Approved = false;

      Result.Set(context, data);
    }
  }
}