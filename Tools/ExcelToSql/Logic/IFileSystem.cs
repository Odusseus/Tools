using System.IO;

namespace ExcelToSql.Logic
{
    public interface IFileSystem
    {
        /// <summary>
        /// Open the read.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The <see cref="FileStream"/>.</returns>
        FileStream OpenRead(string path);
    }
}