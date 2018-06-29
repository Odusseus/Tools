using System.Reflection;

namespace RestClient.Logic
{
    public interface IAssemblyLoader
    {
        Assembly GetEntryAssembly();
    }
}