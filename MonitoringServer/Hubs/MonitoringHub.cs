using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MonitoringServer.Models;

namespace MonitoringServer.Hubs
{
    
    public class MonitoringHub : Hub
    {
        private static object _intervalLocker = new object();
        private static int _interval = 11;
        public async Task SendInterval(int interval)
        {
            lock (_intervalLocker)
                _interval = interval;
            await Clients.All.SendAsync("RefreshTimeChanged", interval);
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
            await Clients.Caller.SendAsync("RefreshTimeChanged", _interval);
        }
    }
}
