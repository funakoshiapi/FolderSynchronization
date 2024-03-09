using System;
using OneWayFolderSync.Models;

namespace OneWayFolderSync.Extensions
{
	public static class PromptExtension
	{
		public static Request InputPrompt()
		{
            Console.WriteLine("Please introduce the source folder path");
            string source = Console.ReadLine();

            Console.WriteLine("Please introduce the destination folder path");
            string destination = Console.ReadLine();

            Console.WriteLine("Please introduce the sycronization interval folder path");
            int syncInterval = int.Parse(Console.ReadLine()) * 1000;

            Console.WriteLine("Please introduce the log file path folder path");
            string logFilePath = Console.ReadLine();

            // perform checks to validate input 

            var request = new Request()
            {
                SourcePath = source,
                DestinationPath = destination,
                SyncInterval = syncInterval,
                LogFilePath = logFilePath
            };

            return request;
        }
    }
	
}

