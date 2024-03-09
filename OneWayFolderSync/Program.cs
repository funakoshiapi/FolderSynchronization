using Serilog;
using OneWayFolderSync.Extensions.ServiceExtensions;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using OneWayFolderSync.Services;
using OneWayFolderSync.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using OneWayFolderSync.Extensions;

namespace OneWayFolderSync
{

    class Program
    {
        static void Main(string[] args)
        {
            // var request = PromptExtension.InputPrompt();

            var request = new Request()
            {
                SourcePath = "/Users/funakoshisilva/Desktop/TestLab",
                DestinationPath = "/Users/funakoshisilva/Desktop/TestLab1",
                SyncInterval = 1,
                LogFilePath = "/Users/funakoshisilva/Desktop/TestLab1/log.txt"
            };

            var host = ServiceExtensions.CreateApplicationHost(request.LogFilePath);

            Log.Information("FolderSync Application - Running");

            var syncService = host.Services.GetRequiredService<ISyncService>();

            while (true)
            {
                
                syncService.RunSyncronization(request);
                Thread.Sleep(request.SyncInterval);

            }
        }

    }
}
