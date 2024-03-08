using System;
namespace OneWayFolderSync.Models
{
	public class Request
	{
		public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public decimal SyncInterval { get; set; }
		public string LogFilePath { get; set; }
	}
}

