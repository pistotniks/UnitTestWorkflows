using System;
using System.Activities;

namespace CustomActivities
{
  public sealed class WaitForData<T> : NativeActivity<T>
  {
    [RequiredArgument] public InArgument<string> BookmarkName { get; set; }

    protected override bool CanInduceIdle => true;

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
      
      Result.Set(context, data);
    }
  }
}