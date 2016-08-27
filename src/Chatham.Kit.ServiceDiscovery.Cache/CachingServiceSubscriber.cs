using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache.Utilities;
using Microsoft.Extensions.Logging;

namespace Chatham.Kit.ServiceDiscovery.Cache
{
    public class CachingServiceSubscriber : IServiceSubscriber, IDisposable
    {
        private readonly ILogger _log;
        private readonly ICacheClient _cache;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly IServiceSubscriber _serviceSubscriber;

        private readonly string _id = Guid.NewGuid().ToString();

        private Task _subscriptionTask;
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        private readonly IThrottle _throttle;

        public string ServiceName => _serviceSubscriber.ServiceName;
        public event EventHandler OnSubscriberChange;

        public CachingServiceSubscriber(ILogger log, IServiceSubscriber serviceSubscriber, ICacheClient cache, IThrottle throttle, CancellationTokenSource cancellationTokenSource)
        {
            _log = log;
            _cache = cache;
            _cancellationTokenSource = cancellationTokenSource;

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
                await _mutex.WaitAsync(_cancellationTokenSource.Token);
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

        private Task SubscriptionLoop()
        {
            return Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        var serviceUris =
                            await await _throttle.Queue(_serviceSubscriber.Endpoints, _cancellationTokenSource.Token);

                        _log.LogDebug($"Received updated endpoints for {ServiceName}");
                        _cache.Set(_id, serviceUris);
                        OnSubscriberChange?.Invoke(this, EventArgs.Empty);
                    }
                    catch (TaskCanceledException)
                    {
                        _log.LogInformation($"Fetching endpoints for {ServiceName} was cancelled.");

                    }
                    catch (Exception ex)
                    {
                        _log.LogError($"Error fetching endpoints for {ServiceName}: {ex}");
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            //TODO:Fix Dispose pattern

            if (_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }

            _cache.Remove(_id);
            _cancellationTokenSource.Dispose();
        }
    }
}
