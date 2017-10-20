using System.Collections.Generic;

namespace ExcelToSql.Logic
{
    public class Header
    {
        public List<Field> Fields { get; set; }

        public Header()
        {
            this.Fields = new List<Field>();
        }
    }
}
