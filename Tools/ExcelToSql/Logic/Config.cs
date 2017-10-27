using System;
using System.Configuration;
using ExcelToSql.Constant;
using ExcelToSql.Enum;

namespace ExcelToSql.Logic
{
    public sealed class Config : IConfig
    {
        public readonly DatabaseEnum.Vendor DatabaseVendor;
        public readonly int OutFileEncoding;
        public readonly int OutStartId;
        public readonly string ExcelFilename;
        public readonly string ExcelPath;
        public readonly string ExcelTabular;
        public readonly string OutCreateFilename;
        public readonly string OutExtraDateFields;
        public readonly string OutExtraFields;
        public readonly string OutExtraNumberFields;
        public readonly string OutInsertFilename;
        public readonly string OutPath;
        public readonly string OutTablename;

        DatabaseEnum.Vendor IConfig.DatabaseVendor { get { return this.DatabaseVendor; } }
        int IConfig.OutFileEncoding { get { return this.OutFileEncoding; } }
        int IConfig.OutStartId { get { return this.OutStartId; } }
        string IConfig.ExcelFilename { get { return this.ExcelFilename; } }
        string IConfig.ExcelPath { get { return this.ExcelPath; } }
        string IConfig.ExcelTabular { get { return this.ExcelTabular; } }
        string IConfig.OutCreateFilename { get { return this.OutCreateFilename; } }
        string IConfig.OutExtraDateFields { get { return this.OutExtraDateFields; } }
        string IConfig.OutExtraFields { get { return this.OutExtraFields; } }
        string IConfig.OutExtraNumberFields { get { return this.OutExtraNumberFields; } }
        string IConfig.OutInsertFilename { get { return this.OutInsertFilename; } }
        string IConfig.OutPath { get { return this.OutPath; } }
        string IConfig.OutTablename { get { return this.OutTablename; } }

        private static volatile Config instance;
        private static object syncRoot = new Object();

        public Config()
        {
            string databaseName = ConfigurationManager.AppSettings[Key.DATABASE_VENDOR];
            this.DatabaseVendor = DatabaseEnum.Vendor.Oracle;

            if(databaseName == Key.POSTGRES)
            {
                this.DatabaseVendor = DatabaseEnum.Vendor.Postgres;
            }
            
            this.ExcelFilename = ConfigurationManager.AppSettings[Key.EXCEL_FILENAME];
            this.ExcelPath = ConfigurationManager.AppSettings[Key.EXCEL_PATH];
            this.ExcelTabular = ConfigurationManager.AppSettings[Key.EXCEL_TABULAR];

            this.OutCreateFilename = ConfigurationManager.AppSettings[Key.OUT_CREATE_FILENAME];
            this.OutInsertFilename = ConfigurationManager.AppSettings[Key.OUT_INSTERT_FILENAME];
            this.OutExtraFields = ConfigurationManager.AppSettings[Key.OUT_EXTRA_FIELDS];
            this.OutExtraNumberFields = ConfigurationManager.AppSettings[Key.OUT_EXTRA_NUMBER_FIELDS];
            this.OutExtraDateFields = ConfigurationManager.AppSettings[Key.OUT_EXTRA_DATE_FIELDS];
            string outFileEncoding = ConfigurationManager.AppSettings[Key.OUT_FILE_ENCODING];

            if (!string.IsNullOrEmpty(outFileEncoding))
            {
                int.TryParse(outFileEncoding, out this.OutFileEncoding);
            }
            if(this.OutFileEncoding == 0)
            {
                this.OutFileEncoding = Key.Utf8;
            }

                        
            this.OutTablename = ConfigurationManager.AppSettings[Key.OUT_TABLENAME].ToLower();
            this.OutPath = ConfigurationManager.AppSettings[Key.OUT_PATH];

            string outStartId = ConfigurationManager.AppSettings[Key.OUT_START_ID];
            int.TryParse(outStartId, out this.OutStartId);
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
