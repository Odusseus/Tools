using System;
using System.Configuration;
using ExcelToSql.Constant;
using ExcelToSql.Enum;

namespace ExcelToSql.Logic
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
        public readonly string OutExtraNumberFields;
        public readonly string OutExtraDateFields;
        public readonly int OutFileEncoding;

        public readonly string OutTablename;
        public readonly string OutPath;
        public readonly int OutStartId;

        private static volatile Config instance;
        private static object syncRoot = new Object();

        internal Config()
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
                int.TryParse(outFileEncoding, out this.OutStartId);
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
