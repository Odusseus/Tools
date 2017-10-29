using System;

namespace ExcelToSql.Logic
{
    public class App : IApp
    {
        public void Exit(int exitCode)
        {
            Environment.Exit(exitCode);
        }
    }
}
