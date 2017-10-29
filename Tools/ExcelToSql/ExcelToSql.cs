using System.Diagnostics.CodeAnalysis;
using ExcelToSql.Constant;
using ExcelToSql.Logic;
using ExcelToSql.Unity;
using Unity;

namespace ExcelToSql
{
    /// <summary>
    /// The excel to sql class.
    /// </summary>
    public class ExcelToSql
    {
        /// <summary>
        /// The main entry point.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [ExcludeFromCodeCoverage]
        public static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();
            ProjectContainer.RegisterElements(container);

            if (GetHelp(args))
            {
                IHelp help = container.Resolve<IHelp>();
                help.Write();
            }
            else
            {
                IGenerateFiles generateFiles = container.Resolve<IGenerateFiles>();
                generateFiles.Run();
            }
        }

        internal static bool GetHelp(string[] args)
        {
            return args.Length == 0 || args[0]?.ToUpper() != Key.GO;
        }
    }
}
