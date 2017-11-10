using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace ExcelToSql.Logic
{
    /// <summary>
    /// The file system class.
    /// </summary>
    [ExcludeFromCodeCoverage]
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

        public void WriteAllLines(string path, List<string> contents, Encoding encoding)
        {
            System.IO.File.WriteAllLines(path, contents, encoding);
        }
    }
}