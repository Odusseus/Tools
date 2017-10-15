using System;
using System.Reflection;

namespace ExcelToSql
{
    public class ExcelToSql
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args[0]?.ToUpper() != Constant.GO)
            {
                Help();
            };

            Config config = Config.Instance;

            GenerateFiles generateFiles = new GenerateFiles(config);

            generateFiles.Run();
        }

        static void Help()
        {
            var version = Assembly.GetEntryAssembly().GetName().Version;
            Console.WriteLine($"ExcelToSql version {version}, 15-10-2017, author Odusseus, https://github.com/Odusseus ");
            Console.WriteLine("ExcelToSql read a Excel spreadsheet and generate basic create and insert sql-scripts.");
            Console.WriteLine("ExcelToSql go          execute the program");
            Console.WriteLine("ExcelToSql             call the help");
            Console.WriteLine("Configuration are in the ExcelToSql.Exe.Config file.");
            Console.WriteLine("- Database.Vendor :     Oracle or Postgres");
            Console.WriteLine("- Excel.Filename  :     Excel filename");
            Console.WriteLine("- Excel.Path :          path to the Excel file");
            Console.WriteLine("- Excel.Tabular :       Name from the tabular");
            Console.WriteLine("- Out.Create.Filename : Output create filename");
            Console.WriteLine("- Out.Insert.Filename : Output insert filename");
            Console.WriteLine("- Out.Path :            Output path");
            Console.WriteLine("- Out.Tablename :       Name from te table");
            Console.WriteLine("- Out.Extra.Fields :    List of extra field to create. The fields are comma separated with optional field length (fielname=fieldlegth).");
            Console.WriteLine("- Exemple :             State,Message=2000");
            Environment.Exit(-1);
        }
    }
}
