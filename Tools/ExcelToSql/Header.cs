using System.Collections.Generic;

namespace ExcelToSql
{
    internal class Header
    {
        public List<Field> Fields { get; set; }

        public Header()
        {
            this.Fields = new List<Field>();
        }
    }
}
