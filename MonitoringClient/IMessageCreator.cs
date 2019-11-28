using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient
{
    interface IMessageCreator
    {
        Task<object> GetMessage();
    }
}
