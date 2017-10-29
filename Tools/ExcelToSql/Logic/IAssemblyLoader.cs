using System.Reflection;

namespace ExcelToSql.Logic
{
    public interface IAssemblyLoader
    {
        Assembly GetEntryAssembly();
    }
}
