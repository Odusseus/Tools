using RestClient.Constant;
using RestClient.Logic;
using RestClient.Unity;
using Unity;

namespace RestClient
{
    public class RestClient
    {
        public static void Main(string[] args)
        {
             IUnityContainer container = new UnityContainer();
            ProjectContainer.RegisterElements(container);

            if (GetHelp(args))
            {
                IHelp help = container.Resolve<IHelp>();
                help.Write();
                System.Environment.Exit(-1);
            }
            else
            {
            }
        }

        internal static bool GetHelp(string[] args)
        {
            return args.Length == 0 || args[0]?.ToUpper() != Key.GO;
        }
    }
}
