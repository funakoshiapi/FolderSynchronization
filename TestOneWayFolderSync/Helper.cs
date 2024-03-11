using System;
namespace TestOneWayFolderSync
{
	public static class Helper
	{
        /// <summary>
        /// Computes the directory byte size
        /// by summing file sizes, including subdirectory files
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static long GetByteSize(string path)
        {
            DirectoryInfo dir1 = new DirectoryInfo(path);

            IEnumerable<FileInfo> list = dir1.GetFiles("*.*", SearchOption.AllDirectories);

            var files = list.Select(f => f)
            .Where(f => (f.Attributes & FileAttributes.Hidden) == 0).ToArray();

            long byteSize = 0;

            foreach (var file in files)
            {
                var byteValue = file.Length;
                byteSize += byteValue;
            }

            return byteSize;
        }
    }
}

