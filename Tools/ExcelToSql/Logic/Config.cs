using System;
using ExcelToSql.Constant;
using ExcelToSql.Enum;

namespace ExcelToSql.Logic
{
    public sealed class Config : IConfig
    {
        IConfigurationManagerLoader configurationManager;

        private readonly DatabaseEnum.Vendor databaseVendor;
        private readonly int outFileEncoding;
        private readonly int outStartId;
        private readonly string excelFilename;
        private readonly string excelPath;
        private readonly string excelTabular;
        private readonly string outCreateFilename;
        private readonly string outExtraDateFields;
        private readonly string outExtraFields;
        private readonly string outExtraNumberFields;
        private readonly string outInsertFilename;
        private readonly string outPath;
        private readonly string outTablename;

        public DatabaseEnum.Vendor DatabaseVendor
        {
            get {
                return this.databaseVendor;
            } }
        public int OutFileEncoding { get { return this.outFileEncoding; } }
        public int OutStartId { get { return this.outStartId; } }
        public string ExcelFilename { get { return this.excelFilename; } }
        public string ExcelPath { get { return this.excelPath; } }
        public string ExcelTabular { get { return this.excelTabular; } }
        public string OutCreateFilename { get { return this.outCreateFilename; } }
        public string OutExtraDateFields { get { return this.outExtraDateFields; } }
        public string OutExtraFields { get { return this.outExtraFields; } }
        public string OutExtraNumberFields { get { return this.outExtraNumberFields; } }
        public string OutInsertFilename { get { return this.outInsertFilename; } }
        public string OutPath { get { return this.outPath; } }
        public string OutTablename { get { return this.outTablename; } }

        private static volatile Config instance;
        private static object syncRoot = new Object();

        public Config(IConfigurationManagerLoader configurationManager)
        {
            this.configurationManager = configurationManager;
            string databaseName = this.configurationManager.KeyDatabaseVendor;
            this.databaseVendor = DatabaseEnum.Vendor.Oracle;

            if(databaseName == Key.POSTGRES)
            {
                this.databaseVendor = DatabaseEnum.Vendor.Postgres;
            }
            
            this.excelFilename = this.configurationManager.KeyExcelFilename;
            this.excelPath = this.configurationManager.KeyExcelPath;
            this.excelTabular = this.configurationManager.KeyExcelTabular;

            this.outCreateFilename = this.configurationManager.KeyOutCreateFilename;
            this.outInsertFilename = this.configurationManager.KeyOutInsertFilename;
            this.outExtraFields = this.configurationManager.KeyOutExtraFields;
            this.outExtraNumberFields = this.configurationManager.KeyOutExtraNumberFields;
            this.outExtraDateFields = this.configurationManager.KeyOutExtraDateFields;
            string outFileEncoding = this.configurationManager.KeyOutFileEncoding;

            if (!string.IsNullOrEmpty(outFileEncoding))
            {
                int.TryParse(outFileEncoding, out this.outFileEncoding);
            }
            if(this.outFileEncoding == 0)
            {
                this.outFileEncoding = Key.Utf8;
            }

                        
            this.outTablename = this.configurationManager.KeyOutTablename?.ToLower();
            this.outPath = this.configurationManager.KeyOutPath;

            string outStartId = this.configurationManager.KeyOutStartId;
            int.TryParse(outStartId, out this.outStartId);
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
                            IConfigurationManagerLoader configurationManagerLoader = new ConfigurationManagerLoader();
                            instance = new Config(configurationManagerLoader);
                        }
                    }
                }
                return instance;
            }
        }
    }
}
