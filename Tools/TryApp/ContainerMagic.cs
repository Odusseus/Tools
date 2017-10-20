using TryApp.Logic;
using Unity;

namespace TryApp
{
    public class ContainerMagic
    {
        public static void RegisterElements(IUnityContainer container)
        {
            //Config config = new Config();
            //container.RegisterInstance(config);

            container.RegisterType<IConfig, Config>();
            container.RegisterType<IRun, Run>();
        }
    }
}
