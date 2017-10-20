using System;

namespace TryApp.Logic
{
    public class Run : IRun
    {
        private readonly IConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Run"/> class.
        /// </summary>
        /// <param name="config">The config.</param>
        public Run(IConfig config)
        {
            this.config = config;
        }
        public bool Go(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                this.config.SetConfig(value);
            }

            Console.WriteLine(this.config.GetConfig());
            return true;
        }
    }
}
