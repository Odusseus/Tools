using System;
using System.Diagnostics.CodeAnalysis;

namespace ExcelToSql.Logic
{
    [ExcludeFromCodeCoverage]
    public class OutputWriter : IOutputWriter
    {
        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }
    }
}