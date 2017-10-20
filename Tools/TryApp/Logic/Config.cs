using System;

namespace TryApp.Logic
{
    public class Config : IConfig
    {
        string value;

        private static volatile Config instance;
        private static object syncRoot = new Object();

        public string GetConfig() => this.value;

        public void SetConfig(string value)
        {
            this.value = value;
        }

        public static Config Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Config();
                        }
                    }
                }
                return instance;
            }
        }
    }
}
