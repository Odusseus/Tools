using System.Data;

namespace ExcelToSql.Logic
{
    public interface ISpreadsheet
    {
        DataTable GetTabular();
    }
}