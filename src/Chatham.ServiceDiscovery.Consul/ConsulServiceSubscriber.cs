using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Chatham.ServiceDiscovery.Abstractions;
using Chatham.ServiceDiscovery.Consul.Internal;
using Chatham.ServiceDiscovery.Consul.Utilities;
using Microsoft.Extensions.Caching.Memory;

namespace Chatham.ServiceDiscovery.Consul
{
    public class ConsulServiceSubscriber : IServiceSubscriber, IDisposable
    {
        private readonly ILogger _log;
        private readonly IMemoryCache _cache;
        private readonly CancellationToken _callerCancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly string _serviceName;
        private readonly IConsulEndpointRetriever _endpointRetriever;

        private readonly string _id = Guid.NewGuid().ToString();

        private Task _subscriptionTask;
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        private readonly Throttle _throttle = new Throttle(5, TimeSpan.FromSeconds(10));

        public ConsulServiceSubscriber(ILogger log, IMemoryCache cache, CancellationTokenSource cancellationTokenSource, 
            CancellationToken callerCancellationToken, string serviceName, IConsulEndpointRetriever endpointRetriever)
        {
            _log = log;
            _cache = cache;
            _cancellationTokenSource = cancellationTokenSource;
            _callerCancellationToken = callerCancellationToken;

            _serviceName = serviceName;
            _endpointRetriever = endpointRetriever;
        }

        public async Task<List<Uri>> EndPoints()
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
                        var serviceUris = await _endpointRetriever.FetchEndpoints();
                        _cache.Set(_id, serviceUris);
                        _subscriptionTask = SubscriptionLoop();
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError($"Error fetching endpoints for {_serviceName}: {ex}");
                }
                finally
                {
                    _mutex.Release();
                }

            }
        }

        private async Task SubscriptionLoop()
        {
            while (!_cancellationTokenSource.IsCancellationRequested && !_callerCancellationToken.IsCancellationRequested)
            {
                try
                {
                    var serviceUris = await await _throttle.Queue(_endpointRetriever.FetchEndpoints, _callerCancellationToken);
                    _cache.Set(_id, serviceUris);
                    _log.LogDebug($"Received updated endpoints for {_serviceName}");
                }
                catch (TaskCanceledException)
                {
                    _cache.Remove(_id);
                }
                catch (Exception ex)
                {
                    _log.LogError($"Error fetching endpoints for {_serviceName}: {ex}");
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
