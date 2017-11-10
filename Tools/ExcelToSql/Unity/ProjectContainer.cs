using System.Diagnostics.CodeAnalysis;
using ExcelToSql.Logic;
using Unity;

namespace ExcelToSql.Unity
{
    [ExcludeFromCodeCoverage]
    public class ProjectContainer
    {
        public static void RegisterElements(IUnityContainer container)
        {
            container.RegisterType<IAssemblyLoader, AssemblyLoader>();
            container.RegisterType<IConfig, Config>();
            container.RegisterType<IConfigurationManagerLoader, ConfigurationManagerLoader>();
            container.RegisterType<IFileSystem, FileSystem>();
            container.RegisterType<IGenerateFiles, GenerateFiles>();
            container.RegisterType<IHelp, Help>();
            container.RegisterType<IOutputWriter, OutputWriter>();
            container.RegisterType<ISpreadsheet, Spreadsheet>();
        }
    }
}