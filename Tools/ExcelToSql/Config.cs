using System;
using System.Configuration;

namespace ExcelToSql
{
    public sealed class Config
    {
        public readonly DatabaseEnum.Vendor DatabaseVendor;
        public readonly string ExcelFilename;
        public readonly string ExcelPath;
        public readonly string ExcelTabular;

        public readonly string OutCreateFilename;
        public readonly string OutInsertFilename;
        public readonly string OutExtraFields;
        public readonly string OutTablename;
        public readonly string OutPath;

        private static volatile Config instance;
        private static object syncRoot = new Object();

        internal Config()
        {
            string databaseName = ConfigurationManager.AppSettings[Constant.DATABASE_VENDOR];
            this.DatabaseVendor = DatabaseEnum.Vendor.Oracle;

            if(databaseName == Constant.POSTGRES)
            {
                this.DatabaseVendor = DatabaseEnum.Vendor.Postgres;
            }
            
            this.ExcelFilename = ConfigurationManager.AppSettings[Constant.EXCEL_FILENAME];
            this.ExcelPath = ConfigurationManager.AppSettings[Constant.EXCEL_PATH];
            this.ExcelTabular = ConfigurationManager.AppSettings[Constant.EXCEL_TABULAR];

            this.OutCreateFilename = ConfigurationManager.AppSettings[Constant.OUT_CREATE_FILENAME];
            this.OutInsertFilename = ConfigurationManager.AppSettings[Constant.OUT_INSTERT_FILENAME];
            this.OutExtraFields = ConfigurationManager.AppSettings[Constant.OUT_EXTRA_FIELDS];
            this.OutTablename = ConfigurationManager.AppSettings[Constant.OUT_TABLENAME];
            this.OutPath = ConfigurationManager.AppSettings[Constant.OUT_PATH];
        }

        public static Config Instance
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
