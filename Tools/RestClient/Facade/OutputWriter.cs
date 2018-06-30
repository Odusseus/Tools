using System;
using System.Diagnostics.CodeAnalysis;

namespace RestClient.Facade
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