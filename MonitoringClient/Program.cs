using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MonitoringClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var hubWorker = new HubWorker<ComputerInfo>(configuration.GetConnectionString("RemoteHUB"))
                .SetMessageCreator(new ComputerInfoCreator())
                .InjectLogWriter(Console.WriteLine);

            Console.WriteLine("Hub starting...");
            try
            {
                await hubWorker.Init();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Hub started.");

            hubWorker.Run();
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }
    }
}
