using System.Collections.Generic;
using System.IO;
using System.Text;

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

        void WriteAllLines(string path, List<string> contents, Encoding encoding);
    }
}