using System;

namespace ExcelToSql.Logic
{
    public class OutputWriter : IOutputWriter
    {
        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }
    }
}
