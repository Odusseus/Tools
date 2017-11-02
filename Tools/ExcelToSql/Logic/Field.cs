using ExcelToSql.Enum;

namespace ExcelToSql.Logic
{
    public class Field
    {
        private string text;
        private string name;

        public int Row { get; set; }
        public int Column { get; set; }

        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                this.text = value.ToString();
                this.name = value.Clean();
            }
        }

        public string Name { get { return name; } }

        public int Length { get; set; }

        public bool Extra { get; set; }

        public DatabaseEnum.TypeField Type { get; set; }
    }
}