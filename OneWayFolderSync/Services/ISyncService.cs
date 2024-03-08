using System;
using OneWayFolderSync.Models;

namespace OneWayFolderSync.Services
{
	public interface ISyncService
	{
		public Request GetSynRequest();
		public void CopyFolder(Request request);
    }
}

