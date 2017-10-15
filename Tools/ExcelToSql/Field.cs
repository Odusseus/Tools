namespace ExcelToSql
{ 
    public class Field
    {
        private string text;

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
                this.Name = value.Trim().ToLower().Replace(" ", "_");
            }
        }
        public string Name { get; set; }

        public int Length { get; set; }

        public bool Extra { get; set; }
        
    }
}
