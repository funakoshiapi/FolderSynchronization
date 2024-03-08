using System;
using Microsoft.Extensions.Logging;
using OneWayFolderSync.Models;

namespace OneWayFolderSync.Services
{
    public class SyncService : ISyncService
	{
		private ILogger _logger;

		public SyncService(ILogger<SyncService> logger)
		{
			_logger = logger;
			_logger.LogInformation("Constructor");
        }

       public void CopyFolder(Request request)
        {
            var directory = new DirectoryInfo(request.SourcePath);
            var destinationDir = request.DestinationPath;

            // goes through all the folder in source directory
            foreach (string dir in Directory.GetDirectories(directory.FullName, "*", SearchOption.AllDirectories))
            {
                string dirToCreate = dir.Replace(directory.FullName, destinationDir);
                if (!Directory.Exists(dirToCreate))
                {
                    Directory.CreateDirectory(dirToCreate);
                    _logger.LogInformation($"Created Dirctory: {dirToCreate}\n");
                }
            }

            foreach (string newPath in Directory.GetFiles(directory.FullName, "*.*", SearchOption.AllDirectories))
            {
                FileInfo srcFile = new FileInfo(newPath);
                FileInfo destFile = new FileInfo(request.DestinationPath + srcFile.FullName.Replace(request.SourcePath, ""));

                if (srcFile.LastWriteTime > destFile.LastWriteTime || !destFile.Exists )
                {

                    File.Copy(srcFile.FullName, destFile.FullName, true);
                    _logger.LogInformation($"Copied File : {destFile.Name} \n to directory {destFile.FullName}\n");
                }
                
            }
        }
        
        public Request GetSynRequest()
        {
            Console.WriteLine("Please introduce the source folder path");
			string source = Console.ReadLine();

            Console.WriteLine("Please introduce the destination folder path");
            string destination = Console.ReadLine();

            Console.WriteLine("Please introduce the sycronization interval folder path");
            string syncInterval = Console.ReadLine();

            Console.WriteLine("Please introduce the log file path folder path");
            string logFilePath = Console.ReadLine();

			// perform checks to validate input 

			var request = new Request()
			{
				SourcePath = source,
				DestinationPath = destination,
				SyncInterval = 0,
				LogFilePath = logFilePath
			};

			return request;
        }
    }
}

