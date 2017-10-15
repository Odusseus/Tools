using System;
using System.IO;
using ExcelDataReader;
using System.Linq;
using System.Data;
using System.Collections.Generic;

namespace ExcelToSql
{
    public class GenerateFiles
    {
        private Config config;

        public GenerateFiles(Config config)
        {
            this.config = config;
        }

        public void Run()
        {
            DataTable tabular = GetTabular();
            Console.WriteLine($"Tabular {config.ExcelTabular} is read.");

            Header header = GetHeader(tabular);
            Console.WriteLine($"Header is read.");

            CreateSqlScript(header);
            Console.WriteLine($"Create file {config.OutCreateFilename} is ready.");

            InsertSqlScript(header, tabular);
            Console.WriteLine($"Insert file {config.OutInsertFilename} is ready.");
        }

        public DataTable GetTabular()
        {
            DataTable tabular;

            string filename = $"{this.config.ExcelPath}\\{this.config.ExcelFilename}";
            using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read))
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

        public void InsertSqlScript(Header header, DataTable tabular)
        {
            string insertFieldNames = string.Empty;
            string valueExtraFields = string.Empty;

            for (int i = 0; i < header.Fields.Count; i++)
            {
                Field field = header.Fields[i];
                insertFieldNames += field.Name;

                if (i < header.Fields.Count - 1)
                {
                    insertFieldNames += ", ";
                }

                if (field.Extra)
                {
                    valueExtraFields += "null";
                    if (i < header.Fields.Count - 1)
                    {
                        valueExtraFields += ", ";
                    }
                }
            }

            int id = 0;
            List<string> inserts = new List<string>();
            foreach (DataRow row in tabular.Rows)
            {
                if (id > 0)
                {
                    string values = string.Empty;
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        var field = row.ItemArray[i];

                        string value = string.Empty;

                        object box = (object)field;
                        if (box.GetType() == typeof(DateTime))
                        {
                            DateTime dateTime = (DateTime)field;
                            value = $"'{dateTime.ToShortDateString()}'";
                        }
                        else
                        {
                            value = $"'{field.ToString().Replace("'","''").Trim()}'";
                        }

                        value += ", ";

                        if (i == row.ItemArray.Length - 1)
                        {
                            value += $"{id}";
                            if (!string.IsNullOrEmpty(valueExtraFields))
                            {
                                value += $", {valueExtraFields}";
                            }
                        }

                        values += value;
                    }
                    string insert = $"INSERT INTO {config.ExcelTabular.ToLower()} ({insertFieldNames}) VALUES ({values});";
                    inserts.Add(insert);
                }
                id++;
            }
            if(config.DatabaseVendor == DatabaseEnum.Vendor.Oracle)
            {
                inserts.Add("COMMIT;");
            }
            File.WriteAllLines($"{config.OutPath}\\{config.OutInsertFilename}", inserts);
        }


        public void CreateSqlScript(Header header)
        {
            List<string> lines = new List<string>();
            string endField = string.Empty;

            for (int i = 0; i < header.Fields.Count; i++)
            {
                Field field = header.Fields[i];

                if (i == 0)
                {
                    lines.Add($"CREATE TABLE {config.ExcelTabular.ToLower()} (");
                }
                if (i < header.Fields.Count - 1)
                {
                    endField = ",";
                }
                else
                {
                    endField = ");";
                }
                if (field.Text == Constant.ID)
                {
                    if (config.DatabaseVendor == DatabaseEnum.Vendor.Oracle)
                    {
                        lines.Add($"{field.Name} NUMBER({field.Length}){endField}");
                    }
                    if (config.DatabaseVendor == DatabaseEnum.Vendor.Postgres)
                    {
                        lines.Add($"{field.Name} BIGINT{endField}");
                    }
                }
                else
                {
                    if(config.DatabaseVendor == DatabaseEnum.Vendor.Oracle)
                    {
                        lines.Add($"{field.Name} VARCHAR2({field.Length}){endField}");
                    }
                    if (config.DatabaseVendor == DatabaseEnum.Vendor.Postgres)
                    {
                        lines.Add($"{field.Name} VARCHAR({field.Length}){endField}");
                    }
                }
            }

            File.WriteAllLines($"{config.OutPath}\\{config.OutCreateFilename}", lines);
        }

        public Header GetHeader(DataTable tabular)
        {
            Header header = new Header();
            int columnId = 0;
            foreach (var item in tabular.Rows[0].ItemArray)
            {
                Field field = new Field
                {
                    Row = 0,
                    Column = columnId,
                    Text = item.ToString().Trim().Replace(" ", "_").Replace("'", "_"),
                    Length = item.ToString().Length,
                };
                header.Fields.Add(field);
                columnId++;
            };

            SetFieldLength(tabular, header);
            AddIdField(header);
            AddExtraFields(header);
            return header;
        }

        public void AddExtraFields(Header header)
        {
            if (!string.IsNullOrEmpty(config.OutExtraFields))
            {
                var outExtraFields = config.OutExtraFields.Split(',');
                foreach (string outExtraField in outExtraFields)
                {
                    Field fieldExtra = new Field
                    {
                        Row = 0,
                        Column = header.Fields.Count,
                        Text = outExtraField,
                        Length = outExtraField.Length,
                        Extra = true
                    };

                    header.Fields.Add(fieldExtra);
                }
            }
        }

        public void AddIdField(Header header)
        {
            Field fieldId = new Field
            {
                Row = 0,
                Column = header.Fields.Count,
                Text = Constant.ID,
                Length = Constant.ID_LENGHT
            };

            header.Fields.Add(fieldId);
        }

        public void SetFieldLength(DataTable tabular, Header header)
        {
            foreach (Field field in header.Fields)
            {
                foreach (DataRow row in tabular.Rows)
                {
                    int length = 0;

                    object box = (object)row.ItemArray[field.Column];
                    if (box.GetType() == typeof(DateTime))
                    {
                        DateTime dateTime = (DateTime)row.ItemArray[field.Column];
                        length = dateTime.ToShortDateString().Length;
                    }
                    else
                    {
                        string text = row.ItemArray[field.Column].ToString();
                        int singlecotes = text.Count(x => x == '\'');

                        length = row.ItemArray[field.Column].ToString().Trim().Length + singlecotes;
                    }

                    if (field.Length < length)
                    {
                        field.Length = length;
                    }
                }
            }
        }
    }
}
