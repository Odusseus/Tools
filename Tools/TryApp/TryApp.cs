using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryApp.Logic;
using Unity;

namespace TryApp
{
    class TryApp
    {
        static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();
            ContainerMagic.RegisterElements(container);

            IRun run = container.Resolve<IRun>();
            bool go1 = run.Go("123");
            bool go2 = run.Go("");

        }
    }
}
