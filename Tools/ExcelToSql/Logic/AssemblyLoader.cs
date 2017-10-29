using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ExcelToSql.Logic
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
