using ExcelToSql.Enum;

namespace ExcelToSql.Logic
{
    public interface IConfig
    {
        DatabaseEnum.Vendor DatabaseVendor { get; }
        int OutFileEncoding { get; }
        int OutStartId { get; }
        string ExcelFilename { get; }
        string ExcelPath { get; }
        string ExcelTabular { get; }
        string OutCreateFilename { get; }
        string OutExtraDateFields { get; }
        string OutExtraFields { get; }
        string OutExtraNumberFields { get; }
        string OutInsertFilename { get; }
        string OutPath { get; }
        string OutTablename { get; }
    }
}
