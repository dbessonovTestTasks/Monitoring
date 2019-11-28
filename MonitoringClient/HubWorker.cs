using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;


namespace MonitoringClient
{
    class HubWorker
    {
        private IMessageCreator _messageCreator;
        private Action<string> _logWriter;
        private string _url;
        private HubConnection _connection;
        private int _delay=1;
        private Timer _timer;

        private async Task SendMessage()
        {
            try
            {
                await _connection.InvokeAsync("SendInfo", await _messageCreator.GetMessage());
            }
            catch (Exception e)
            {
                _logWriter?.Invoke(e.Message);
            }
            finally
            {
                _timer.Change(_delay * 1000, Timeout.Infinite);
            }
        }

        public HubWorker(string url)
        {
            _url = url;
        }

        public HubWorker SetMessageCreator(IMessageCreator messageCreator)
        {
            _messageCreator = messageCreator;
            return this;
        }

        public HubWorker InjectLogWriter(Action<string> logWriter)
        {
            _logWriter = logWriter;
            return this;
        }

        public async Task Init()
        {
            _connection = new HubConnectionBuilder().WithUrl(_url).Build();
             _connection.Closed += async (error) =>
            {
                await Task.Delay(_delay);
                await _connection.StartAsync();
            };

             _connection.On<int>("RefreshTimeChanged", param =>
             {
                 _delay = param;
             });
            await _connection.StartAsync();
            await _connection.InvokeAsync("AddToGroup", "ConsoleClients");

            _timer = new Timer(async (o) => await SendMessage());
        }

        public async void Run()
        {
            await SendMessage();
        }
    }
}
