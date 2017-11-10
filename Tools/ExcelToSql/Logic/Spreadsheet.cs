using System.Data;
using System.Linq;
using ExcelDataReader;

namespace ExcelToSql.Logic
{
    public class Spreadsheet : ISpreadsheet
    {
        private IConfig config;
        private IFileSystem fileSystem;

        public Spreadsheet(IConfig config, IFileSystem fileSystem)
        {
            this.config = config;
            this.fileSystem = fileSystem;
        }

        public DataTable GetTabular()
        {
            DataTable tabular;

            string filename = $"{this.config.ExcelPath}\\{this.config.ExcelFilename}";
            using (var stream = this.fileSystem.OpenRead(filename))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var dataset = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        // Gets or sets a value indicating whether to set the DataColumn.DataType
                        // property in a second pass.
                        UseColumnDataType = true,

                        // Gets or sets a callback to obtain configuration options for a DataTable.
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            // Gets or sets a value indicating the prefix of generated column names.
                            EmptyColumnNamePrefix = "Column",

                            // Gets or sets a value indicating whether to use a row from the
                            // data as column names.
                            UseHeaderRow = false,

                            // Gets or sets a callback to determine which row is the header row.
                            // Only called when UseHeaderRow = true.
                            ReadHeaderRow = (rowReader) =>
                            {
                                // F.ex skip the first row and use the 2nd row as column headers:
                                rowReader.Read();
                            },

                            // Gets or sets a callback to determine whether to include the
                            // current row in the DataTable.
                            FilterRow = (rowReader) =>
                            {
                                return true;
                            },
                        }
                    });

                    tabular = dataset.Tables.Cast<DataTable>().FirstOrDefault(d => d.TableName == config.ExcelTabular);
                }
            }

            return tabular;
        }
    }
}