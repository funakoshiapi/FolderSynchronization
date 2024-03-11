using OneWayFolderSync.Extensions.ServiceExtensions;
using Microsoft.Extensions.DependencyInjection;
using OneWayFolderSync.Services;
using OneWayFolderSync.Extensions;
using System.IO;
using Serilog;

namespace OneWayFolderSync
{

    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var request = PromptExtension.InputPrompt();

                var host = ServiceExtensions.CreateApplicationHost(request.LogFilePath);

                var syncService = host.Services.GetRequiredService<ISyncService>();
     
                

                while (true)
                {
                    await syncService.RunSyncronization(request);
                    Thread.Sleep(request.SyncInterval);
                    
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"General exception: '{e}'");
                Console.ReadLine();
            }

        }
    }
}
