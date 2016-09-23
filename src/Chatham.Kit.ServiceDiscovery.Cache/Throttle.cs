using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Cache
{
    public class Throttle : IThrottle
    {
        private readonly TimeSpan _maxPeriod;
        private readonly SemaphoreSlim _throttleActions, _throttlePeriods;

        public Throttle(int maxActions, TimeSpan maxPeriod)
        {
            _throttleActions = new SemaphoreSlim(maxActions, maxActions);
            _throttlePeriods = new SemaphoreSlim(maxActions, maxActions);
            _maxPeriod = maxPeriod;
        }

        public Task<T> Queue<T>(Func<T> action, CancellationToken cancel)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Queue));
            }

            return _throttleActions.WaitAsync(cancel).ContinueWith(t =>
            {
                try
                {
                    _throttlePeriods.Wait(cancel);

                    // Release after period
                    // - Allow bursts up to maxActions requests at once
                    // - Do not allow more than maxActions requests per period
                    Task.Delay(_maxPeriod, cancel).ContinueWith(tt =>
                    {
                        _throttlePeriods.Release(1);
                    }, cancel).ConfigureAwait(false);

                    return action();
                }
                finally
                {
                    _throttleActions.Release(1);
                }
            }, cancel);
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _throttleActions.Dispose();
                    _throttlePeriods.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
