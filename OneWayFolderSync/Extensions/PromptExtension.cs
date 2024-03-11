using System;
using System.IO;
using OneWayFolderSync.Models;

namespace OneWayFolderSync.Extensions
{
    /// <summary>
    /// This class promts the user to input a request order, that will be used by the SyncService.
    /// </summary>
	public static class PromptExtension
	{
        /// <summary>
        /// Input prompt
        /// </summary>
        /// <returns>Returns a request object</returns>
		public static Request InputPrompt()
		{
            Console.WriteLine("Please introduce the source folder path:");
            var source = GetPath();
            Console.WriteLine("\n");

            Console.WriteLine("Please introduce the destination folder path:");
            string destination = GetPath();
            Console.WriteLine("\n");

            Console.WriteLine("Please introduce the sycronization interval:");
            int syncInterval = GetSyncInterval();
            Console.WriteLine("\n");

            Console.WriteLine("Please introduce the log file path folder path:");
            string logFilePath = GetPath();
            Console.WriteLine("\n");

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

        /// <summary>
        /// Gets path and perform validation
        /// </summary>
        /// <returns>returns a string that represents a path</returns>
        private static string GetPath()
        {
            string source = Console.ReadLine();

            if (Directory.Exists(source))
            {
                return source;
            }
            else
            {
                Console.WriteLine("Please introduce a valid path:\n");
                GetPath();
            }

            return source;
        }

        /// <summary>
        /// Gets sync interval and performs validation
        /// </summary>
        /// <returns>return an int representing a millisecond interval</returns>
        private static int GetSyncInterval()
        {
            int interval = 1;
            string syncInterval = Console.ReadLine();
            try
            {
                interval = int.Parse(syncInterval) * 1000;
                return interval;
            }
            catch
            {
                Console.WriteLine("Please introduce a valid integer value:\n");
                GetSyncInterval();
            }
            return interval;
        }

    }
	
}

