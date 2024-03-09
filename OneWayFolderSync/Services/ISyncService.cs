using System;
using OneWayFolderSync.Models;

namespace OneWayFolderSync.Services
{
	public interface ISyncService
	{
		public void RunSyncronization(Request request);
    }
}

