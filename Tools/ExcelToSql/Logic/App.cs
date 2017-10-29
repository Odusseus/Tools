using System;
using System.Diagnostics.CodeAnalysis;

namespace ExcelToSql.Logic
{
    [ExcludeFromCodeCoverage]
    public class App : IApp
    {
        public void Exit(int exitCode)
        {
            Environment.Exit(exitCode);
        }
    }
}
