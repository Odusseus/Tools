namespace ExcelToSql.Logic
{
    public interface IConfigurationManagerLoader
    {
        string KeyDatabaseVendor { get; }
        string KeyExcelFilename { get; }
        string KeyExcelPath { get; }
        string KeyExcelTabular { get; }
        string KeyOutCreateFilename { get; }
        string KeyOutInsertFilename { get; }
        string KeyOutExtraFields { get; }
        string KeyOutExtraNumberFields { get; }
        string KeyOutExtraDateFields { get; }
        string KeyOutFileEncoding { get; }
        string KeyOutTablename { get; }
        string KeyOutPath { get; }
        string KeyOutStartId { get; }
    }
}