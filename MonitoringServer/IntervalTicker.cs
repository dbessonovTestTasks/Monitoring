using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MonitoringServer.Hubs;

namespace MonitoringServer
{
    public class IntervalTicker
    {
        private readonly object _changeIntervalLock = new object();
        private readonly int _checkInterval = 1000;
        private readonly ConcurrentStack<int> _newIntervals = new ConcurrentStack<int>();
        private readonly IHubClients _clients;
        private readonly Timer _timer;

        public int CurrentInterval { get; private set; } = 10;

        public IntervalTicker(IHubContext<MonitoringHub> hubContext)
        {
            _clients = hubContext.Clients;
            _timer = new Timer(TrySendInterval);
            _timer.Change(_checkInterval, Timeout.Infinite);
        }

        private void TrySendInterval(object state)
        {
            lock (_changeIntervalLock)
            {
                if (_newIntervals.TryPop(out var newInterval))
                {
                    _newIntervals.Clear();
                    CurrentInterval = newInterval;
                }
            }
            _clients.All.SendAsync("RefreshTimeChanged", CurrentInterval);
            _timer.Change(_checkInterval, Timeout.Infinite);
        }

        public void AddInterval(int newInterval)
        {
            lock (_changeIntervalLock)
            {
                _newIntervals.Push(newInterval);
            }
        }
    }
}
