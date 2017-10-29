using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ExcelToSql.Logic
{
    [ExcludeFromCodeCoverage]
    public class Row
    {
        public List<Element> Elements { get; set; }
    }
}