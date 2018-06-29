namespace RestClient.Logic
{
    public class Help : IHelp
    {
        private readonly IAssemblyLoader assemblyLoader;
        private readonly IOutputWriter outputWriter;

        public Help(IAssemblyLoader assemblyLoader, IOutputWriter outputWriter)
        {
            this.assemblyLoader = assemblyLoader;
            this.outputWriter = outputWriter;
        }

        public void Write()
        {
            var version = this.assemblyLoader.GetEntryAssembly().GetName().Version;
            this.outputWriter.WriteLine($"RestClient version {version}, 29-06-2018, author Odusseus, https://github.com/Odusseus ");
            this.outputWriter.WriteLine("RestClient go          execute the program");
            this.outputWriter.WriteLine("RestClient             call the help");
            this.outputWriter.WriteLine("Configuration are in the RestClient.Exe.Config file.");            
        }
    }
}