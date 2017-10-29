using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ExcelToSql.Logic
{
    [ExcludeFromCodeCoverage]
    public class Header
    {
        public List<Field> Fields { get; set; }

        public Header()
        {
            this.Fields = new List<Field>();
        }
    }
}
