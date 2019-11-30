using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient
{
    class ComputerInfoCreator : IMessageCreator<ComputerInfo>
    {
        public async Task<ComputerInfo> GetMessage()
        {
            var memoryMetrics = MemoryMetricsClient.GetMetrics();
            var compInfo = new ComputerInfo(memoryMetrics.Total, memoryMetrics.Free, await GetCpuUsageForProcess());

            foreach (DriveInfo d in DriveInfo.GetDrives())
                compInfo.AddHddInfo(d.Name, d.TotalSize / 1024 / 1024, d.TotalFreeSpace / 1024 / 1024);

            return compInfo;
        }

        //PerformanceCounter не можем использовать.
        //К защищенным процессам так доступ тоже не получим
        //Изначальный список процессов может не соответствовать конечному
        //Да и TotalProcessorTime может уменьшиться
        //В общем, считается так, но лучше сделать через PerformanceCounter для win, и тот или иной велосипед для *nix
        private TimeSpan GetTotalProcessorTime()
        {
            var cpuUsage = TimeSpan.Zero;
            foreach (var process in Process.GetProcesses())
                try
                {
                    cpuUsage += process.TotalProcessorTime;
                }
                catch (Win32Exception e)
                {
                   //Нет привилегий, обработать нельзя. Спамить в консоль смысла нет, ситуация ожидаемая.
                }

            return cpuUsage;
        }

        private async Task<double> GetCpuUsageForProcess()
        {
            var startTime = DateTime.UtcNow;
            TimeSpan startCpuUsage = GetTotalProcessorTime();
            await Task.Delay(500);
            
            var endTime = DateTime.UtcNow;
            var endCpuUsage = GetTotalProcessorTime();

            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return Math.Round(cpuUsageTotal * 100, 2);
        }
    }
}
