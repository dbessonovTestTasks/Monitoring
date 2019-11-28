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

    class ComputerInfoCreator:IMessageCreator
    {
        public async Task<object> GetMessage()
        {
            var memoryMetrics = MemoryMetricsClient.GetMetrics();
            var compInfo = new ComputerInfo(memoryMetrics.Total, memoryMetrics.Free, await GetCpuUsageForProcess());

            foreach (DriveInfo d in DriveInfo.GetDrives())
                compInfo.AddHddInfo(d.Name, d.TotalSize / 1024 / 1024, d.TotalFreeSpace / 1024 / 1024);

            return compInfo;
        }

        private async Task<double> GetCpuUsageForProcess()
        {
            var startTime = DateTime.UtcNow;
            TimeSpan startCpuUsage = TimeSpan.Zero;
            foreach (var process in Process.GetProcesses())
                startCpuUsage += Process.GetCurrentProcess().TotalProcessorTime;

            await Task.Delay(500);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = TimeSpan.Zero;
            foreach (var process in Process.GetProcesses())
                endCpuUsage += Process.GetCurrentProcess().TotalProcessorTime;
            
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return Math.Round(cpuUsageTotal * 100, 2);
        }
    }
}
