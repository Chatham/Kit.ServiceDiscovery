using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chatham.Kit.ServiceDiscovery.Abstractions;
using Chatham.Kit.ServiceDiscovery.Cache.Utilities;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Chatham.Kit.ServiceDiscovery.Cache
{
    public class CacheServiceSubscriber : IServiceSubscriber, IDisposable
    {
        private bool _disposed;

        private readonly ILogger _log;
        private readonly ICacheClient _cache;
        private readonly CancellationTokenSource _cts;

        private readonly IServiceSubscriber _serviceSubscriber;

        private readonly string _id = Guid.NewGuid().ToString();

        private Task _subscriptionTask;
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        private readonly IThrottle _throttle;

        public string ServiceName => _serviceSubscriber.ServiceName;
        public event EventHandler OnSubscriberChange;

        public CacheServiceSubscriber(ILogger log, IServiceSubscriber serviceSubscriber, ICacheClient cache, IThrottle throttle, CancellationTokenSource cts)
        {
            _log = log;
            _cache = cache;
            _cts = cts;

            _serviceSubscriber = serviceSubscriber;
            _throttle = throttle;
        }

        public async Task<List<ServiceEndpoint>> Endpoints()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(CacheServiceSubscriber));
            }

            await StartSubscription();

            return _cache.Get<List<ServiceEndpoint>>(_id);
        }

        private async Task StartSubscription()
        {
            if (_subscriptionTask == null)
            {
                await _mutex.WaitAsync(_cts.Token);
                try
                {
                    if (_subscriptionTask == null)
                    {
                        var serviceUris = await _serviceSubscriber.Endpoints().ConfigureAwait(false);
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
                var previousEndpoints = new List<ServiceEndpoint>();
                while (!_cts.IsCancellationRequested)
                {
                    _log.LogTrace($"Iteration of subscription loop for {ServiceName}.");
                    try
                    {
                        var currentEndpoints =
                            await await _throttle.Queue(_serviceSubscriber.Endpoints, _cts.Token)
                                .ConfigureAwait(false);

                        if (!CompareEndpoints(previousEndpoints, currentEndpoints))
                        {
                            _log.LogDebug($"Received updated endpoints for {ServiceName}");
                            _cache.Set(_id, currentEndpoints);
                            OnSubscriberChange?.Invoke(this, EventArgs.Empty);
                            previousEndpoints = currentEndpoints;
                        }
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
            }, _cts.Token);
        }

        private static bool CompareEndpoints(List<ServiceEndpoint> endpoints1, List<ServiceEndpoint> endpoints2)
        {
            if (endpoints1.Count != endpoints2.Count) return false;

            var filteredSequence = endpoints1.Where(endpoints2.Contains);
            return filteredSequence.Count() == endpoints1.Count;
        }

        ~CacheServiceSubscriber()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_cts.IsCancellationRequested)
                {
                    _cts.Cancel();
                }
                _cts.Dispose();
            }

            _cache.Remove(_id);

            _disposed = true;
        }
    }
}
