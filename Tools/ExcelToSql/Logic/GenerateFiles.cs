using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ExcelToSql.Constant;
using ExcelToSql.Enum;

namespace ExcelToSql.Logic
{
    public class GenerateFiles : IGenerateFiles
    {
        private readonly IConfig config;
        private readonly ISpreadsheet spreadsheet;
        private readonly IFileSystem fileSystem;

        public GenerateFiles(IConfig config, ISpreadsheet spreadsheet, IFileSystem fileSystem)
        {
            this.config = config;
            this.spreadsheet = spreadsheet;
            this.fileSystem = fileSystem;
        }

        public bool Run()
        {
            DataTable tabular = spreadsheet.GetTabular();

            Console.WriteLine($"Tabular {config.ExcelTabular} is read.");

            Header header = GetHeader(tabular);
            Console.WriteLine($"Header is read.");

            CreateSqlScript(header);
            Console.WriteLine($"Create file {config.OutCreateFilename} is ready.");

            int inserts = InsertSqlScript(header, tabular);
            Console.WriteLine($"Insert {inserts} rows in file {config.OutInsertFilename} is ready.");

            return true;
        }

        internal Header GetHeader(DataTable tabular)
        {
            Header header = new Header();
            int columnId = 0;
            if(tabular.Rows.Count == 0)
            {
                return header;
            }

            foreach (var item in tabular.Rows[0].ItemArray)
            {
                Field field = new Field
                {
                    Row = 0,
                    Column = columnId,
                    Text = item.ToString(),
                    Length = item.ToString().Length.RoundUp(),
                    Type = DatabaseEnum.TypeField.Text
                };
                header.Fields.Add(field);
                columnId++;
            };

            SetFieldLength(tabular, header);
            AddIdField(header);
            AddExtraFields(header);
            AddExtraNumberFields(header);
            AddExtraDateFields(header);
            return header;
        }

        internal void SetFieldLength(DataTable tabular, Header header)
        {
            if(tabular == null || header == null)
            {
                return;
            }

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
                        length = (row.ItemArray[field.Column].ToString().Length).RoundUp();
                    }

                    if (field.Length < length)
                    {
                        field.Length = length;
                    }
                }
            }
        }

        internal void AddIdField(Header header)
        {
            Field fieldId = new Field
            {
                Row = 0,
                Column = header.Fields.Count,
                Text = Key.ID,
                Length = Key.ID_LENGHT,
                Type = DatabaseEnum.TypeField.Number
            };

            header.Fields.Add(fieldId);
        }

        internal void AddExtraFields(Header header)
        {
            if (!string.IsNullOrEmpty(config.OutExtraFields))
            {
                var outExtraFields = config.OutExtraFields.Split(',');
                foreach (string outExtraField in outExtraFields)
                {
                    string[] extraFields = outExtraField.Split('=');
                    string extraField = extraFields[0];
                    int extraFieldLength = extraField.Length;
                    if (extraFields.Length == 2)
                    {
                        if (!int.TryParse(extraFields[1], out extraFieldLength))
                        {
                            extraFieldLength = extraField.Length;
                        };
                    }

                    Field fieldExtra = new Field
                    {
                        Row = 0,
                        Column = header.Fields.Count,
                        Text = extraField,
                        Length = extraFieldLength.RoundUp(),
                        Extra = true,
                        Type = DatabaseEnum.TypeField.Text
                    };

                    header.Fields.Add(fieldExtra);
                }
            }
        }

        internal void AddExtraNumberFields(Header header)
        {
            if (!string.IsNullOrEmpty(config.OutExtraNumberFields))
            {
                var outExtraNumberFields = config.OutExtraNumberFields.Split(',');
                foreach (string outExtraNumberField in outExtraNumberFields)
                {
                    string[] extraNumberFields = outExtraNumberField.Split('=');
                    string extraNumberField = extraNumberFields[0];
                    int extraNumberFieldLength = extraNumberField.Length;
                    if (extraNumberFields.Length == 2)
                    {
                        if (!int.TryParse(extraNumberFields[1], out extraNumberFieldLength))
                        {
                            extraNumberFieldLength = extraNumberField.Length;
                        };
                    }

                    Field fieldExtra = new Field
                    {
                        Row = 0,
                        Column = header.Fields.Count,
                        Text = extraNumberField,
                        Length = extraNumberFieldLength,
                        Extra = true,
                        Type = DatabaseEnum.TypeField.Number
                    };

                    header.Fields.Add(fieldExtra);
                }
            }
        }

        internal void AddExtraDateFields(Header header)
        {
            if (!string.IsNullOrEmpty(config.OutExtraDateFields))
            {
                var outExtraDateFields = config.OutExtraDateFields.Split(',');
                foreach (string outExtraDateField in outExtraDateFields)
                {
                    Field fieldExtra = new Field
                    {
                        Row = 0,
                        Column = header.Fields.Count,
                        Text = outExtraDateField,
                        Length = 20,
                        Extra = true,
                        Type = DatabaseEnum.TypeField.Date
                    };

                    header.Fields.Add(fieldExtra);
                }
            }
        }

        internal void CreateSqlScript(Header header)
        {
            List<string> lines = new List<string>();
            string endField = string.Empty;

            for (int i = 0; i < header.Fields.Count; i++)
            {
                Field field = header.Fields[i];

                if (i == 0)
                {
                    lines.Add($"CREATE TABLE {config.OutTablename} (");
                }
                if (i < header.Fields.Count - 1)
                {
                    endField = ",";
                }
                else
                {
                    endField = ");";
                }
                if (field.Type == DatabaseEnum.TypeField.Number)
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
                else if (field.Type == DatabaseEnum.TypeField.Date)
                {
                    if (config.DatabaseVendor == DatabaseEnum.Vendor.Oracle)
                    {
                        lines.Add($"{field.Name} DATE{endField}");
                    }
                    if (config.DatabaseVendor == DatabaseEnum.Vendor.Postgres)
                    {
                        lines.Add($"{field.Name} Date{endField}");
                    }
                }
                else if (field.Type == DatabaseEnum.TypeField.Text)
                {
                    if (config.DatabaseVendor == DatabaseEnum.Vendor.Oracle)
                    {
                        lines.Add($"{field.Name} VARCHAR2({field.Length}){endField}");
                    }
                    if (config.DatabaseVendor == DatabaseEnum.Vendor.Postgres)
                    {
                        lines.Add($"{field.Name} VARCHAR({field.Length}){endField}");
                    }
                }
            }

            if (lines.Count > 0)
            {
                if (config.DatabaseVendor == DatabaseEnum.Vendor.Oracle || config.DatabaseVendor == DatabaseEnum.Vendor.Postgres)
                {
                    lines.Add("");
                    lines.Add($"CREATE UNIQUE INDEX {config.OutTablename}_pk_index ON {config.OutTablename} ({Key.ID.ToLower()});");
                }
            }

            this.fileSystem.WriteAllLines($"{config.OutPath}\\{config.OutCreateFilename}", lines, Encoding.GetEncoding(config.OutFileEncoding));
        }

        internal int InsertSqlScript(Header header, DataTable tabular)
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

            int id = -1; // First row must be 0
            List<string> inserts = new List<string>();
            foreach (DataRow row in tabular.Rows)
            {
                id++;

                if (id == 0)
                {
                    // First row is the header, do nothing.
                }
                else
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
                            value = $"'{field.ToString().Replace("'", "''").Trim()}'";
                        }

                        value += ", ";

                        if (i == row.ItemArray.Length - 1)
                        {
                            value += $"{config.OutStartId + id}";
                            if (!string.IsNullOrEmpty(valueExtraFields))
                            {
                                value += $", {valueExtraFields}";
                            }
                        }

                        values += value;
                    }
                    string insert = $"INSERT INTO {config.OutTablename} ({insertFieldNames}) VALUES ({values});";
                    inserts.Add(insert);
                }
            }
            if (config.DatabaseVendor == DatabaseEnum.Vendor.Oracle)
            {
                inserts.Add("");
                inserts.Add($"{Constant.Key.COMMIT};");
            }

            this.fileSystem.WriteAllLines($"{config.OutPath}\\{config.OutInsertFilename}", inserts, Encoding.GetEncoding(config.OutFileEncoding));

            return id;
        }
    }
}