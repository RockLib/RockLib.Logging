using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public interface IAsyncWaitHandle : IDisposable
    {
        Task WaitAsync(
            int millisecondsTimeout = Timeout.Infinite,
            CancellationToken cancellationToken = default(CancellationToken));

        void Release(int releaseCount = 1);
    }
}