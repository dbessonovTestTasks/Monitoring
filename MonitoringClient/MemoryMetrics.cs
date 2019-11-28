using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace MonitoringClient
{
    public class MemoryMetrics
    {
        public int Total;
        public int Free;
    }

    public class MemoryMetricsClient
    {
        public static MemoryMetrics GetMetrics()
        {
            MemoryMetrics metrics;

            if (IsUnix())
            {
                metrics = GetUnixMetrics();
            }
            else
            {
                metrics = GetWindowsMetrics();
            }

            return metrics;
        }

        private static bool IsUnix()
        {
            var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                         RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            return isUnix;
        }

        private static MemoryMetrics GetWindowsMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo();
            info.FileName = "wmic";
            info.Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value";
            info.RedirectStandardOutput = true;

            using (var process = Process.Start(info))
            {
                output = process.StandardOutput.ReadToEnd();
            }

            var lines = output.Trim().Split("\n");
            var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
            var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

            var metrics = new MemoryMetrics();
            metrics.Total = (int)Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0);
            metrics.Free = (int)Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0);

            return metrics;
        }

        private static MemoryMetrics GetUnixMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo("free -m");
            info.FileName = "/bin/bash";
            info.Arguments = "-c \"free -m\"";
            info.RedirectStandardOutput = true;

            using (var process = Process.Start(info))
            {
                output = process.StandardOutput.ReadToEnd();
                Console.WriteLine(output);
            }

            var lines = output.Split("\n");
            var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var metrics = new MemoryMetrics();
            metrics.Total = (int)double.Parse(memory[1]);
            metrics.Free = (int)double.Parse(memory[3]);

            return metrics;
        }
    }
}
