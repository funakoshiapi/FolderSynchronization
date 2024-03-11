using System;
using OneWayFolderSync.Models;

namespace OneWayFolderSync.Services
{
	public interface ISyncService
	{
		public void RunSyncronization(Request request);
		public void CopyAllDirectories(Request request);
        public void CopyFiles(Request request);
        public void DeleteFilesSubdirectory(DirectoryInfo source, DirectoryInfo destination);
        public void DeleteFiles(DirectoryInfo source, DirectoryInfo destination);
    }
}

