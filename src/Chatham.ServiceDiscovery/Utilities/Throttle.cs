using System;
using System.Threading;
using System.Threading.Tasks;

namespace Chatham.ServiceDiscovery.Utilities
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
                    }, cancel);

                    return action();
                }
                finally
                {
                    _throttleActions.Release(1);
                }
            }, cancel);
        }
    }
}
