using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MonitoringServer.Models;

namespace MonitoringServer.Hubs
{
    
    public class MonitoringHub : Hub
    {
        private static IntervalTicker _ticker;

        public MonitoringHub(IntervalTicker ticker)
        {
            _ticker = ticker;
        }

        public void SendInterval(int interval)
        {
            _ticker.AddInterval(interval);
        }

        public async Task SendInfo(ComputerInfo data)
        {
            data.ClientIp = Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
            DbWorker.SaveInfo(data);
            await Clients.Groups("webClients").SendAsync("ReceiveInfo", data);
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("RefreshTimeChanged", _ticker.CurrentInterval);
        }
    }
}
