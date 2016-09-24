using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chatham.Kit.ServiceDiscovery.Cache
{
    public class Throttle : IThrottle
    {
        private bool _disposed;

        private readonly TimeSpan _maxPeriod;
        private readonly SemaphoreSlim _throttleActions, _throttlePeriods;

        public Throttle(int maxActions, TimeSpan maxPeriod)
        {
            _throttleActions = new SemaphoreSlim(maxActions, maxActions);
            _throttlePeriods = new SemaphoreSlim(maxActions, maxActions);
            _maxPeriod = maxPeriod;
        }

        public Task<T> Queue<T>(Func<T> action, CancellationToken ct)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Queue));
            }

            return _throttleActions.WaitAsync(ct).ContinueWith(t =>
            {
                try
                {
                    _throttlePeriods.Wait(ct);

                    // Release after period
                    // - Allow bursts up to maxActions requests at once
                    // - Do not allow more than maxActions requests per period
                    Task.Delay(_maxPeriod, ct).ContinueWith(tt =>
                    {
                        _throttlePeriods.Release(1);
                    }, ct).ConfigureAwait(false);

                    return action();
                }
                finally
                {
                    _throttleActions.Release(1);
                }
            }, ct);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _throttleActions.Dispose();
                _throttlePeriods.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
