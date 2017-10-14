using System;
using System.Configuration;

namespace ExcelToSql
{
    internal sealed class Config
    {
        internal readonly DatabaseEnum.Database DatabaseName;
        internal readonly string ExcelFilename;
        internal readonly string ExcelPath;
        internal readonly string ExcelTabular;

        internal readonly string OutCreateFilename;
        internal readonly string OutInsertFilename;
        internal readonly string OutPath;
        internal readonly string OutExtraFields;

        private static volatile Config instance;
        private static object syncRoot = new Object();

        private Config()
        {
            string databaseName = ConfigurationManager.AppSettings[Constant.DATABASE_NAME];
            this.DatabaseName = DatabaseEnum.Database.Oracle;

            if(databaseName == Constant.POSTGRES)
            {
                this.DatabaseName = DatabaseEnum.Database.Postgres;
            }


            this.ExcelFilename = ConfigurationManager.AppSettings[Constant.EXCEL_FILENAME];
            this.ExcelPath = ConfigurationManager.AppSettings[Constant.EXCEL_PATH];
            this.ExcelTabular = ConfigurationManager.AppSettings[Constant.EXCEL_TABULAR];

            this.OutCreateFilename = ConfigurationManager.AppSettings[Constant.OUT_CREATE_FILENAME];
            this.OutInsertFilename = ConfigurationManager.AppSettings[Constant.OUT_INSTERT_FILENAME];
            this.OutPath = ConfigurationManager.AppSettings[Constant.OUT_PATH];
            this.OutExtraFields = ConfigurationManager.AppSettings[Constant.OUT_EXTRA_FIELDS];
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
