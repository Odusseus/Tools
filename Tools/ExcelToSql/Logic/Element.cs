using System.Diagnostics.CodeAnalysis;

namespace ExcelToSql.Logic
{
    [ExcludeFromCodeCoverage]
    public class Element
    {
        public Field Name { get; set; }
        public Field Value { get; set; }
    }
}
