using System;
using System.Configuration;

namespace ExcelToSql
{
    internal sealed class Config
    {
        internal readonly string Filename;
        internal readonly string Path;
        internal readonly string Tabular;

        private static volatile Config instance;
        private static object syncRoot = new Object();

        private Config()
        {
            this.Filename = ConfigurationManager.AppSettings[Constant.FILENAME];
            this.Path = ConfigurationManager.AppSettings[Constant.PATH];
            this.Tabular = ConfigurationManager.AppSettings[Constant.TABULAR];
        }

        internal static Config Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if(instance == null)
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
