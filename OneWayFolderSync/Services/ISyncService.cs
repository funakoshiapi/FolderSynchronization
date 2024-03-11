using System;
using OneWayFolderSync.Models;

namespace OneWayFolderSync.Services
{
    /// <summary>
    /// SyncService interface
    /// </summary>
	public interface ISyncService
	{
		public  Task RunSyncronization(Request request);
		public Task CopyAllDirectories(Request request);
        public Task CopyFiles(Request request);
        public void DeleteFilesSubdirectory(DirectoryInfo source, DirectoryInfo destination);
        public void DeleteFiles(DirectoryInfo source, DirectoryInfo destination);
    }
}

