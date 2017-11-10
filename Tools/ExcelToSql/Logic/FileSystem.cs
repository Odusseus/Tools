using System.IO;

namespace ExcelToSql.Logic
{
    /// <summary>
    /// The file system class.
    /// </summary>
    public class FileSystem : IFileSystem
    {
        /// <summary>
        /// Open the read.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The <see cref="FileStream"/>.</returns>
        public FileStream OpenRead(string path)
        {
            return System.IO.File.OpenRead(path);
        }
    }
}
