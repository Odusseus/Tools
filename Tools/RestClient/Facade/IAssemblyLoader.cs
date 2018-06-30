using System.Reflection;

namespace RestClient.Facade
{
    public interface IAssemblyLoader
    {
        Assembly GetEntryAssembly();
    }
}