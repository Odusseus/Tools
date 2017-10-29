using System;
using ExcelToSql.Constant;
using ExcelToSql.Enum;

namespace ExcelToSql.Logic
{
    public sealed class Config : IConfig
    {
        IConfigurationManagerLoader configurationManager;

        private readonly DatabaseEnum.Vendor _DatabaseVendor;
        private readonly int _OutFileEncoding;
        private readonly int _OutStartId;
        private readonly string _ExcelFilename;
        private readonly string _ExcelPath;
        private readonly string _ExcelTabular;
        private readonly string _OutCreateFilename;
        private readonly string _OutExtraDateFields;
        private readonly string _OutExtraFields;
        private readonly string _OutExtraNumberFields;
        private readonly string _OutInsertFilename;
        private readonly string _OutPath;
        private readonly string _OutTablename;

        public DatabaseEnum.Vendor DatabaseVendor
        {
            get {
                return this._DatabaseVendor;
            } }
        public int OutFileEncoding { get { return this._OutFileEncoding; } }
        public int OutStartId { get { return this._OutStartId; } }
        public string ExcelFilename { get { return this._ExcelFilename; } }
        public string ExcelPath { get { return this._ExcelPath; } }
        public string ExcelTabular { get { return this._ExcelTabular; } }
        public string OutCreateFilename { get { return this._OutCreateFilename; } }
        public string OutExtraDateFields { get { return this._OutExtraDateFields; } }
        public string OutExtraFields { get { return this._OutExtraFields; } }
        public string OutExtraNumberFields { get { return this._OutExtraNumberFields; } }
        public string OutInsertFilename { get { return this._OutInsertFilename; } }
        public string OutPath { get { return this._OutPath; } }
        public string OutTablename { get { return this._OutTablename; } }

        private static volatile Config instance;
        private static object syncRoot = new Object();

        public Config(IConfigurationManagerLoader configurationManager)
        {
            this.configurationManager = configurationManager;
            string databaseName = this.configurationManager.KeyDatabaseVendor;
            this._DatabaseVendor = DatabaseEnum.Vendor.Oracle;

            if(databaseName == Key.POSTGRES)
            {
                this._DatabaseVendor = DatabaseEnum.Vendor.Postgres;
            }
            
            this._ExcelFilename = this.configurationManager.KeyExcelFilename;
            this._ExcelPath = this.configurationManager.KeyExcelPath;
            this._ExcelTabular = this.configurationManager.KeyExcelTabular;

            this._OutCreateFilename = this.configurationManager.KeyOutCreateFilename;
            this._OutInsertFilename = this.configurationManager.KeyOutInsertFilename;
            this._OutExtraFields = this.configurationManager.KeyOutExtraFields;
            this._OutExtraNumberFields = this.configurationManager.KeyOutExtraNumberFields;
            this._OutExtraDateFields = this.configurationManager.KeyOutExtraDateFields;
            string outFileEncoding = this.configurationManager.KeyOutFileEncoding;

            if (!string.IsNullOrEmpty(outFileEncoding))
            {
                int.TryParse(outFileEncoding, out this._OutFileEncoding);
            }
            if(this._OutFileEncoding == 0)
            {
                this._OutFileEncoding = Key.Utf8;
            }

                        
            this._OutTablename = this.configurationManager.KeyOutTablename?.ToLower();
            this._OutPath = this.configurationManager.KeyOutPath;

            string outStartId = this.configurationManager.KeyOutStartId;
            int.TryParse(outStartId, out this._OutStartId);
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
