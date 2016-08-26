using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chatham.ServiceDiscovery.Abstractions;
using Chatham.ServiceDiscovery.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Chatham.ServiceDiscovery
{
    public class CachingServiceSubscriber : IServiceSubscriber, IDisposable
    {
        private readonly ILogger _log;
        private readonly IMemoryCache _cache;
        private readonly CancellationToken _callerCancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly IServiceSubscriber _serviceSubscriber;

        private readonly string _id = Guid.NewGuid().ToString();

        private Task _subscriptionTask;
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        private readonly IThrottle _throttle;

        public string ServiceName => _serviceSubscriber.ServiceName;
        public event EventHandler OnSubscriberChange;

        public CachingServiceSubscriber(ILogger log, IServiceSubscriber serviceSubscriber, IMemoryCache cache, IThrottle throttle, CancellationTokenSource cancellationTokenSource, CancellationToken callerCancellationToken)
        {
            _log = log;
            _cache = cache;
            _cancellationTokenSource = cancellationTokenSource;
            _callerCancellationToken = callerCancellationToken;

            _serviceSubscriber = serviceSubscriber;
            _throttle = throttle;
        }

        public async Task<List<Uri>> Endpoints()
        {
            await StartSubscription();

            return _cache.Get<List<Uri>>(_id);
        }

        private async Task StartSubscription()
        {
            if (_subscriptionTask == null)
            {
                await _mutex.WaitAsync(_callerCancellationToken);
                try
                {
                    if (_subscriptionTask == null)
                    {
                        var serviceUris = await _serviceSubscriber.Endpoints();
                        _cache.Set(_id, serviceUris);
                        _subscriptionTask = SubscriptionLoop();
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError($"Error fetching endpoints for {ServiceName}: {ex}");
                }
                finally
                {
                    _mutex.Release();
                }

            }
        }

        private async Task SubscriptionLoop()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var serviceUris = await await _throttle.Queue(_serviceSubscriber.Endpoints, _callerCancellationToken);

                    _log.LogDebug($"Received updated endpoints for {ServiceName}");
                    _cache.Set(_id, serviceUris);
                    OnSubscriberChange?.Invoke(this, EventArgs.Empty);
                }
                catch (TaskCanceledException)
                {
                    _cache.Remove(_id);
                }
                catch (Exception ex)
                {
                    _log.LogError($"Error fetching endpoints for {ServiceName}: {ex}");
                }
            }

            if (_callerCancellationToken.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public void Dispose()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }
    }
}
