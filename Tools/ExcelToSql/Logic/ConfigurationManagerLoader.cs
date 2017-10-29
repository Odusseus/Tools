using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using ExcelToSql.Constant;

namespace ExcelToSql.Logic
{
    [ExcludeFromCodeCoverage]
    public class ConfigurationManagerLoader : IConfigurationManagerLoader
    {
        public string KeyDatabaseVendor
        {
            get{
                return ConfigurationManager.AppSettings[Key.DATABASE_VENDOR];
              }
        }

        public string KeyExcelFilename
        {
            get
            {
                return ConfigurationManager.AppSettings[Key.EXCEL_FILENAME];
            }
        }
        public string KeyExcelPath
        {
            get
            {
                return ConfigurationManager.AppSettings[Key.EXCEL_PATH];
            }
        }
        public string KeyExcelTabular
        {
            get
            {
                return ConfigurationManager.AppSettings[Key.EXCEL_TABULAR];
            }
        }
        public string KeyOutCreateFilename
        {
            get
            {
                return ConfigurationManager.AppSettings[Key.OUT_CREATE_FILENAME];
            }
        }
        public string KeyOutInsertFilename
        {
            get
            {
                return ConfigurationManager.AppSettings[Key.OUT_INSTERT_FILENAME];
            }
        }
        public string KeyOutExtraFields
        {
            get
            {
                return ConfigurationManager.AppSettings[Key.OUT_EXTRA_FIELDS];
            }
        }
        public string KeyOutExtraNumberFields
        {
            get
            {
                return ConfigurationManager.AppSettings[Key.OUT_EXTRA_NUMBER_FIELDS];
            }
        }
        public string KeyOutExtraDateFields
        {
            get
            {
                return ConfigurationManager.AppSettings[Key.OUT_EXTRA_DATE_FIELDS];
            }
        }
        public string KeyOutFileEncoding
        {
            get
            {
                return ConfigurationManager.AppSettings[Key.OUT_FILE_ENCODING];
            }
        }
        public string KeyOutTablename
        {
            get
            {
                return ConfigurationManager.AppSettings[Key.OUT_TABLENAME];
            }
        }
        public string KeyOutPath
        {
            get
            {
                return ConfigurationManager.AppSettings[Key.OUT_PATH];
            }
        }
        public string KeyOutStartId
        {
            get
            {
                return ConfigurationManager.AppSettings[Key.OUT_START_ID];
            }
        }
    }
}