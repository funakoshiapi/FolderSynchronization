using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneWayFolderSync.Services;
using Serilog;

namespace TestOneWayFolderSync
{
	public static class ConfigureServices
	{
	
            public static IHost CreateApplicationHost()
            {
                var builder = new ConfigurationBuilder();
                BuildConfig(builder);

                ConfigureLogger();

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

            public static void ConfigureLogger()
            {
                Log.Logger = new LoggerConfiguration()
               .WriteTo.Console()
               .CreateLogger();
            }



    }
}
	


