using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using OneWayFolderSync.Models;


namespace OneWayFolderSync.Services
{
    /// <summary>
    ///  SyncService class performs one directional folder syncronization
    ///  target (destination folder) will be similar to source folder.
    ///  Meaning deleted, copied or created files in source folder will
    ///  refelct in target(destination folder)
    /// </summary>
    public class SyncService : ISyncService
    {
        private readonly ILogger _logger;
 

        public SyncService(ILogger<SyncService> logger)
		{
			_logger = logger;
        }

        /// <summary>
        /// Executes the syncronization process, preriodicatly given a Request object
        /// </summary>
        /// <param name="request" Representes a syncronization request</param>
        public async Task RunSyncronization(Request request)
        {
            var directory = new DirectoryInfo(request.SourcePath);
            var destinationDir = new DirectoryInfo(request.DestinationPath);

            await CopyFolder(request);
            Delete(directory, destinationDir);
                
            
        }

        /// <summary>
        ///  Performs Deletion of files, directories and files in subdirectories
        /// </summary>
        /// <param name="source">Directory info object, describing the source directory</param>
        /// <param name="destination">Directory info object, describing the destination directory</param>
        private void Delete(DirectoryInfo source, DirectoryInfo destination)
        {
            if (!source.Exists)
            {
                Directory.Delete(destination.FullName, true);
                _logger.LogWarning($"Deleted - Directory: {destination.Name}");
                return;
            }

            DeleteFiles(source, destination);
            DeleteFilesSubdirectory(source, destination);
        }


        /// <summary>
        ///   Delete non existing files in each subdirectory using recursion.
        /// </summary>
        /// <param name="source">Directory info object, describing the source directory</param>
        /// <param name="destination">Directory info object, describing the destination directory</param>
        public void DeleteFilesSubdirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            foreach (DirectoryInfo destinationSubDir in destination.GetDirectories())
            {
                DirectoryInfo sourceSubDir = new DirectoryInfo(Path.Combine(source.FullName, destinationSubDir.Name));
                Delete(sourceSubDir, destinationSubDir);
            }
        }

        /// <summary>
        ///   Delete files existing in destination folder that do not exist in source folder
        /// </summary>
        /// <param name="source">Directory info object, describing the source directory</param>
        /// <param name="destination">Directory info object, describing the destination directory</param>
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

        /// <summary>
        /// Copies files including files in subdirectories
        /// </summary>
        /// <param name="request">Request Object</param>
        public async Task CopyFiles(Request request)
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
                    await Task.Run(() => File.Copy(srcFile.FullName, destFile.FullName, true));
                  
                    if (destFile.Name != "Log.txt" && !destFile.Exists)
                    {
                        _logger.LogInformation($"Copied File : {destFile.Name} \n to directory {destFile.FullName}\n");
                    }
                }

            }
        }

        /// <summary>
        /// Copies all directories
        /// </summary>
        /// <param name="request">represents an request object</param>
        public async Task CopyAllDirectories(Request request)
        {
            var directory = new DirectoryInfo(request.SourcePath);
            var destinationDir = request.DestinationPath;

            // goes through all the folder in source directory

            foreach (string dir in Directory.GetDirectories(directory.FullName, "*", SearchOption.AllDirectories))
            {
                string dirToCreate = dir.Replace(directory.FullName, destinationDir);
                if (!Directory.Exists(dirToCreate))
                {
                     await Task.Run(() => Directory.CreateDirectory(dirToCreate));
                    _logger.LogInformation($"Created Dirctory: {dirToCreate}\n");
                }
            }
        }

        /// <summary>
        /// Executs the copying process that includes both copting directories and all files including subdirectories
        /// </summary>
        /// <param name="request">Represents a request object</param>
        private async Task CopyFolder(Request request)
        {
            await CopyAllDirectories(request);
            await CopyFiles(request);
        }


    }
}

