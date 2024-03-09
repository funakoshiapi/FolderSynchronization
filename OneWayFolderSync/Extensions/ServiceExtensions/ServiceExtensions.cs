using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneWayFolderSync.Models;
using OneWayFolderSync.Services;
using Serilog;

namespace OneWayFolderSync.Extensions.ServiceExtensions
{
    public static class ServiceExtensions
    {
        public static IHost CreateApplicationHost(string logFilePath)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            ConfigureLogger(logFilePath);

            var host = Host.CreateDefaultBuilder()
              .ConfigureServices((context, services) =>
              {
                  services.AddSingleton<ISyncService, SyncService>();
              })
              .UseSerilog()
              .Build(); 

            return host;
        }

        public static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddEnvironmentVariables();
        }

        public static void ConfigureLogger(string logPath)
        {
           Log.Logger = new LoggerConfiguration()
          .WriteTo.Console() // Output the logs to the console
          .WriteTo.File(logPath)
          .CreateLogger();
        }



    }
}
