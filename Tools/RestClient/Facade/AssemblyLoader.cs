using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace RestClient.Facade
{
    [ExcludeFromCodeCoverage]
    public class AssemblyLoader : IAssemblyLoader
    {
        public Assembly GetEntryAssembly()
        {
            return Assembly.GetEntryAssembly();
        }
    }
}
