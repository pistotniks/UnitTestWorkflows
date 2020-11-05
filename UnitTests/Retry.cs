using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Polly;
using Polly.Retry;

namespace UnitTests
{
  public class Retry
  {
    public static RetryPolicy<bool> WaitUntil(TextWriter progressTracker)
    {
      var policy = Policy.HandleResult<bool>(false)
        .WaitAndRetry(
          new List<int> {10, 50, 100, 200, 300, 500, 1000, 4000}
            .Select(o => TimeSpan.FromMilliseconds(o)),
          (res, attempt) =>
          {
            try
            {
              progressTracker.WriteLine(
                $"Retrying the verification, attempt: '{attempt}', {res.Result}");
            }
            catch (Exception e)
            {
              progressTracker.WriteLine(e);
            }
          });

      return policy;
    }
  }
}