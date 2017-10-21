using ExcelToSql.Logic;
using Unity;

namespace ExcelToSql.Unity
{
    public class ProjectContainer
    {
        public static void RegisterElements(IUnityContainer container)
        {
            container.RegisterType<IConfig, Config>();
            container.RegisterType<IGenerateFiles, GenerateFiles>();

        }
    }
}
