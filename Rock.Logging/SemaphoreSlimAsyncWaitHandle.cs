using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rock.Logging
{
    public class SemaphoreSlimAsyncWaitHandle : IAsyncWaitHandle
    {
        private readonly SemaphoreSlim _semaphore;

        public SemaphoreSlimAsyncWaitHandle()
        {
            _semaphore = new SemaphoreSlim(1);
        }

        Task IAsyncWaitHandle.WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return _semaphore.WaitAsync(millisecondsTimeout, cancellationToken);
        }

        void IAsyncWaitHandle.Release(int releaseCount)
        {
            _semaphore.Release(releaseCount);
        }

        void IDisposable.Dispose()
        {
            _semaphore.Dispose();
        }
    }
}