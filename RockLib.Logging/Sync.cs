using System;
using System.Threading;
using System.Threading.Tasks;

namespace RockLib.Logging
{
    internal static class Sync
    {
        public static void OverAsync(Func<Task> getTaskOfTResult)
        {
            SynchronizationContext old = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                getTaskOfTResult().GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(old);
            }
        }
    }
}
