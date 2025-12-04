using System.Collections.Generic;
using System.IO;
using System.Linq;

//Wholesale changes to file structure.  Converted everything to static class methods.  Minor refactoring.
namespace LlamaSoft.NET.Extensions
{
    public static class IO
    {
        public static List<FileInfo> GetFileInfos(this DirectoryInfo directory, string pattern, SearchOption searchoption, ushort limit = ushort.MinValue)
        {
            List<FileInfo> files = directory.GetFiles(pattern, searchoption).ToList<FileInfo>();
            if (files.Any() && limit > 0)
            {
                return files.TakeWhile(x => x.Exists).ToList<FileInfo>();
            }
            return files;
        }
    }
}
