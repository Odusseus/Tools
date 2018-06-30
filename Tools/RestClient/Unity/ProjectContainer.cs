using System.Diagnostics.CodeAnalysis;
using RestClient.Logic;
using RestClient.Facade;
using Unity;

namespace RestClient.Unity
{
    [ExcludeFromCodeCoverage]
    public class ProjectContainer
    {
        public static void RegisterElements(IUnityContainer container)
        {
            container.RegisterType<IAssemblyLoader, AssemblyLoader>();
            container.RegisterType<IHelp, Help>();
            container.RegisterType<IOutputWriter, OutputWriter>();
            container.RegisterType<IBaseClient, BaseClient>();
        }
    }
}