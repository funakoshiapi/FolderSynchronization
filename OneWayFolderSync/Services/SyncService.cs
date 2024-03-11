using System.IO;
using Microsoft.Extensions.Logging;
using OneWayFolderSync.Models;


namespace OneWayFolderSync.Services
{
    public class SyncService : ISyncService
    {
        private readonly ILogger _logger;
 

        public SyncService(ILogger<SyncService> logger)
		{
			_logger = logger;
        }

        public void RunSyncronization(Request request)
        {
            var directory = new DirectoryInfo(request.SourcePath);
            var destinationDir = new DirectoryInfo(request.DestinationPath);

            while (true)
            {
                Delete(directory, destinationDir);
                CopyFolder(request);
                Thread.Sleep(request.SyncInterval);
            }
        }

        private void Delete(DirectoryInfo source, DirectoryInfo destination)
        {
            if (!source.Exists)
            {
                Directory.Delete(destination.FullName, true);
                _logger.LogWarning($"Deleted - Directory: {destination.Name}");
                return;
            }

            // Delete files existing in destination folder that do not exist in source folder
            DeleteFiles(source, destination);

            // Delete non existing files in each subdirectory using recursion.
            DeleteFilesSubdirectory(source, destination);
        }

        public void DeleteDirectory(DirectoryInfo destination)
        {
                Directory.Delete(destination.FullName, true);
                _logger.LogWarning($"Deleted - Directory: {destination.Name}");
            
        }

        public void DeleteFilesSubdirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            foreach (DirectoryInfo destinationSubDir in destination.GetDirectories())
            {
                DirectoryInfo sourceSubDir = new DirectoryInfo(Path.Combine(source.FullName, destinationSubDir.Name));
                Delete(sourceSubDir, destinationSubDir);
            }
        }

        public void DeleteFiles(DirectoryInfo source,DirectoryInfo destination)
        {
            // Existing file in destination directory gets deleted if not existing in the source directory
            foreach (FileInfo fi in destination.GetFiles())
            {
                var sourceFile = Path.Combine(source.FullName, fi.Name);
                // if Source file doesn't exist, delete destination file
                if (!File.Exists(sourceFile)) 
                {
                    fi.Delete();
                    _logger.LogWarning($"Deleted - File: {fi.Name}");
                }
            }
        }

        public void CopyFiles(Request request)
        {
            var directory = new DirectoryInfo(request.SourcePath);

            foreach (string newPath in Directory.GetFiles(directory.FullName, "*.*", SearchOption.AllDirectories))
            {
                FileInfo srcFile = new FileInfo(newPath);
                FileInfo destFile = new FileInfo(request.DestinationPath + srcFile.FullName.Replace(request.SourcePath, ""));

                if (srcFile.CreationTime == srcFile.LastAccessTime && !destFile.Exists)
                {
                    _logger.LogInformation($"Created File : {srcFile.Name} \n");

                }

                if (srcFile.LastWriteTime > destFile.LastWriteTime || !destFile.Exists)
                {
                    File.Copy(srcFile.FullName, destFile.FullName, true);

                    if (destFile.Name != "Log.txt")
                    {
                        _logger.LogInformation($"Copied File : {destFile.Name} \n to directory {destFile.FullName}\n");
                    }
                }

            }
        }

        public void CopyAllDirectories(Request request)
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
        }

        private void CopyFolder(Request request)
        {
            CopyAllDirectories(request);
            CopyFiles(request);
        }


    }
}

