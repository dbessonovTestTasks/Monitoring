using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitoringServer.Models
{
    public class ComputerInfo
    {
        public string ClientIp { get; set; }
        public int TotalRAM { get; set; }
        public int FreeRAM { get; set; }
        public double CPULoad { get; set; }
        public HDDInfo[] HDDInfo { get; set; } 
    }

    public class HDDInfo
    {
        public string HDDName { get; set; }
        public long HDDTotalSpace { get; set; }
        public long HDDFreeSpace { get; set; }
    }
}
