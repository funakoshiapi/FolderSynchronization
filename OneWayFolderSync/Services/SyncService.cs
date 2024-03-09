using System;
using Microsoft.Extensions.Logging;
using OneWayFolderSync.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OneWayFolderSync.Services
{
    public class SyncService : ISyncService
    {
        private ILogger _logger;
 

        public SyncService(ILogger<SyncService> logger)
		{
			_logger = logger;
        }


        public void RunSyncronization(Request request)
        {
            var directory = new DirectoryInfo(request.SourcePath);
            var destinationDir = new DirectoryInfo(request.DestinationPath);

            Delete(directory, destinationDir);
            CopyFolder(request);


        }

        private void Delete(DirectoryInfo source, DirectoryInfo destination)
        {
            if (!source.Exists)
            {
                destination.Delete(true);
                _logger.LogWarning($"Deleted - Directory: {destination.Name}");
                return;
            }

            // Delete each existing file in destination directory not existing in the source directory.
            foreach (FileInfo fi in destination.GetFiles())
            {
                var sourceFile = Path.Combine(source.FullName, fi.Name);
                if (!File.Exists(sourceFile)) //Source file doesn't exist, delete destination file
                {
                    fi.Delete();
                    _logger.LogWarning($"Deleted - File: {fi.Name}");
                }
            }

            // Delete non existing files in each subdirectory using recursion.
            foreach (DirectoryInfo destinationSubDir in destination.GetDirectories())
            {
                DirectoryInfo sourceSubDir = new DirectoryInfo(Path.Combine(source.FullName, destinationSubDir.Name));
                Delete(sourceSubDir, destinationSubDir);
            }
        }


        private void CopyFolder(Request request)
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



                if (srcFile.CreationTime == srcFile.LastAccessTime && !destFile.Exists)
                {
                    _logger.LogInformation($"Created File : {srcFile.Name} \n");
                    
                }
                if (srcFile.LastWriteTime > destFile.LastWriteTime || !destFile.Exists )
                {

                    File.Copy(srcFile.FullName, destFile.FullName, true);
                    _logger.LogInformation($"Copied File : {destFile.Name} \n to directory {destFile.FullName}\n");
                }
                
            }
        }


    }
}

