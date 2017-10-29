using System.Reflection;

namespace ExcelToSql.Logic
{
    public class Help : IHelp
    {
        readonly IApp app;
        readonly IAssemblyLoader assemblyLoader;
        readonly IOutputWriter outputWriter;

        public Help(IAssemblyLoader assemblyLoader, IApp app, IOutputWriter outputWriter)
        {
            this.assemblyLoader = assemblyLoader;
            this.app = app;
            this.outputWriter = outputWriter;
        }

        public void Write()
        {
            var version = this.assemblyLoader.GetEntryAssembly().GetName().Version;
            this.outputWriter.WriteLine($"ExcelToSql version {version}, 15-10-2017, author Odusseus, https://github.com/Odusseus ");
            this.outputWriter.WriteLine("ExcelToSql read a Excel spreadsheet and generate basic create and insert sql-scripts.");
            this.outputWriter.WriteLine("ExcelToSql go          execute the program");
            this.outputWriter.WriteLine("ExcelToSql             call the help");
            this.outputWriter.WriteLine("Configuration are in the ExcelToSql.Exe.Config file.");
            this.outputWriter.WriteLine("- Database.Vendor :     Oracle or Postgres");
            this.outputWriter.WriteLine("- Excel.Filename  :     Excel filename");
            this.outputWriter.WriteLine("- Excel.Path :          path to the Excel file");
            this.outputWriter.WriteLine("- Excel.Tabular :       Name from the tabular");
            this.outputWriter.WriteLine("- Out.Create.Filename : Output create filename");
            this.outputWriter.WriteLine("- Out.Insert.Filename : Output insert filename");
            this.outputWriter.WriteLine("- Out.Path :            Output path");
            this.outputWriter.WriteLine("- Out.Tablename :       Name from te table");
            this.outputWriter.WriteLine("- Out.Extra.Fields :    List of extra field to create. The fields are comma separated with optional field length (fielname=fieldlegth).");
            this.outputWriter.WriteLine("- Exemple :             State,Message=2000");
            this.app.Exit(-1);
        }
    }
}
