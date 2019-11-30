using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient
{
    class ComputerInfo
    {
        public int TotalRAM { get; set; }
        public int FreeRAM { get; set; }
        public double CPULoad { get; set; }

        public List<HDDInfo> HDDInfo { get; } = new List<HDDInfo>();

        public ComputerInfo(int totalRam, int freeRam, double cpuLoad)
        {
            TotalRAM = totalRam;
            FreeRAM = freeRam;
            CPULoad = cpuLoad;
        }
        public void AddHddInfo(string HDDName, long HDDTotalSpace, long HDDFreeSpace)
        {
            HDDInfo.Add(new HDDInfo(HDDName, HDDTotalSpace, HDDFreeSpace));
        }
    }

    class HDDInfo
    {
        public string HDDName { get; set; }
        public long HDDTotalSpace { get; set; }
        public long HDDFreeSpace { get; set; }

        public HDDInfo(string hddName, long hddTotalSpace, long hddFreeSpace)
        {
            HDDName = hddName;
            HDDTotalSpace = hddTotalSpace;
            HDDFreeSpace = hddFreeSpace;
        }
    }
}
