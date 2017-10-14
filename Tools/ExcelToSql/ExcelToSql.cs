using System;
using System.Reflection;

namespace ExcelToSql
{
    class ExcelToSql
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args[0]?.ToUpper() != Constant.GO)
            {
                Help();
            };

            Config config = Config.Instance;

            Read read = new Read(config);

            read.Run();
        }

        static void Help()
        {
            var version = Assembly.GetEntryAssembly().GetName().Version;
            Console.WriteLine($"ExcelToSql version {version}");
            Console.WriteLine("ExcelToSql read a Excel spreadsheet and generate sql-scripts to create and insert values in a database.");
            Console.WriteLine("ExcelToSql go : execute the program");
            Console.WriteLine("ExcelToSql go : execute the program");
            Console.WriteLine("Configuration are in ExcelToSql.Exe.Config");
            Console.WriteLine("- Filename : Excel filename");
            Console.WriteLine("- Path : path to the Excel file");
            Console.WriteLine("- Tabular : Tabular's name");
            Environment.Exit(-1);
        }
    }
}
