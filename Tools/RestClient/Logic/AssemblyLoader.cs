using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace RestClient.Logic
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
