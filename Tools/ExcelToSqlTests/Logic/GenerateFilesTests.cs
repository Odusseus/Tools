using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace ExcelToSql.Logic.Tests
{
    [TestClass()]
    [ExcludeFromCodeCoverage]
    public class GenerateFilesTests
    {
        private Mock<IConfig> configMock;
        private IConfig config;

        private Mock<ISpreadsheet> spreadsheetMock;
        private ISpreadsheet spreadsheet;

        private Mock<IFileSystem> fileSystemMock;
        private IFileSystem fileSystem;

        [TestInitialize]
        public void TestInitialize()
        {
            this.configMock = new Mock<IConfig>();
            this.config = configMock.Object;

            this.spreadsheetMock = new Mock<ISpreadsheet>();
            this.spreadsheet = spreadsheetMock.Object;

            this.fileSystemMock = new Mock<IFileSystem>();
            this.fileSystem = fileSystemMock.Object;
        }

        [TestMethod()]
        public void RunTest_Return_True()
        {
            // Arrange

            //string tabularString = JsonConvert.SerializeObject(tabular).Replace("\"","\\\"");

            string spreadsheetString = "[{\"Column0\":\"OrderDate\",\"Column1\":\"Region's\",\"Column2\":\"Rep\",\"Column3\":\"Item\",\"Column4\":\"Units\",\"Column5\":\"Unit Cost\",\"Column6\":\"Total\"},{\"Column0\":\"2016-01-06T00:00:00\",\"Column1\":\"East's\",\"Column2\":\"Jones\",\"Column3\":\"Pencil\",\"Column4\":95.0,\"Column5\":1.99,\"Column6\":189.05}]".Replace("\\", "");
            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(spreadsheetString);

            spreadsheetMock.Setup(s => s.GetTabular()).Returns(dataTable);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem);

            // Act
            bool result = generateFiles.Run();

            // Assert
            result.Should().BeTrue();
            spreadsheetMock.Verify(s => s.GetTabular(), Times.Once);
        }

        [TestMethod()]
        public void GetHeaderTest_Should_Return_Header_With_8_Fields()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem);

            string spreadsheetString = "[{\"Column0\":\"OrderDate\",\"Column1\":\"Region's\",\"Column2\":\"Rep\",\"Column3\":\"Item\",\"Column4\":\"Units\",\"Column5\":\"Unit Cost\",\"Column6\":\"Total\"},{\"Column0\":\"2016-01-06T00:00:00\",\"Column1\":\"East's\",\"Column2\":\"Jones\",\"Column3\":\"Pencil\",\"Column4\":95.0,\"Column5\":1.99,\"Column6\":189.05}]".Replace("\\", "");
            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(spreadsheetString);

            // Act
            Header header = generateFiles.GetHeader(dataTable);

            // Assert
            header.Should().NotBeNull();
            header.Fields.Count.Should().Be(8);
        }

        [TestMethod()]
        public void GetHeaderTest_Should_Return_Header_With_0_Fields()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem);

            //string spreadsheetString = "[{\"Column0\":\"OrderDate\",\"Column1\":\"Region's\",\"Column2\":\"Rep\",\"Column3\":\"Item\",\"Column4\":\"Units\",\"Column5\":\"Unit Cost\",\"Column6\":\"Total\"},{\"Column0\":\"2016-01-06T00:00:00\",\"Column1\":\"East's\",\"Column2\":\"Jones\",\"Column3\":\"Pencil\",\"Column4\":95.0,\"Column5\":1.99,\"Column6\":189.05}]".Replace("\\", "");
            //DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(spreadsheetString);
            DataTable dataTable = new DataTable();

            // Act
            Header header = generateFiles.GetHeader(dataTable);

            // Assert
            header.Should().NotBeNull();
            header.Fields.Count.Should().Be(0);
        }

        [TestMethod()]
        public void SetFieldLength_Should_Let_DataTable_And_Null_When_There_Are_Null()
        {
            // Arrange
            DataTable dataTable = null;
            Header header = null;

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem);

            // Act
            generateFiles.SetFieldLength(dataTable, header);

            // Assert
            dataTable.Should().BeNull();
            header.Should().BeNull();
        }

        [TestMethod()]
        public void SetFieldLength_Should_Not_Change_Hasch_From_DataTable_When_Header_Is_Null()
        {
            // Arrange
            DataTable dataTable = new DataTable();
            int dataTableHash = dataTable.GetHashCode();
            Header header = null;

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem);

            // Act
            generateFiles.SetFieldLength(dataTable, header);

            // Assert
            dataTable.GetHashCode().Should().Be(dataTableHash);
        }

        [TestMethod()]
        public void SetFieldLength_Should_Let_DataTable_Null_When_It_Is_Null()
        {
            // Arrange
            DataTable dataTable = new DataTable();
            int dataTableHash = dataTable.GetHashCode();
            Header header = null;

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem);

            // Act
            generateFiles.SetFieldLength(dataTable, header);

            // Assert
            dataTable.GetHashCode().Should().Be(dataTableHash);
        }

        [TestMethod()]
        public void SetFieldLength_Should_Set_The_Length_Followed_The_Field_Length()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem);

            DataTable dataTable = GetPeopleTest1();

            int dataTableHash = dataTable.GetHashCode();
            Header header = new Header
            {
                Fields = new List<Field>
                {
                    new Field
                    {
                        Column = 0,
                        Text = "LastName",
                        Length = 0,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Text
                    },
                    new Field
                    {
                        Column = 1,
                        Text = "FirstName",
                        Length = 0,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Text
                    },
                    new Field
                    {
                        Column = 2,
                        Text = "Name",
                        Length = 4,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Text
                    },
                    new Field
                    {
                        Column = 3,
                        Text = "DateOfBirth",
                        Length = 0,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Date
                    },
                    new Field
                    {
                        Column = 4,
                        Text = "Salary",
                        Length = 0,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Number
                    }
                }
            };

            // Act
            generateFiles.SetFieldLength(dataTable, header);

            // Assert
            header.Should().NotBeNull();
            header.Fields.Find(f => f.Name == "LastName".ToLower()).Length.Should().Be(20);
            header.Fields.Find(f => f.Name == "FirstName".ToLower()).Length.Should().Be(30);
            header.Fields.Find(f => f.Name == "Name".ToLower()).Length.Should().Be(10);
            header.Fields.Find(f => f.Name == "DateOfBirth".ToLower()).Length.Should().Be(10);
            header.Fields.Find(f => f.Name == "Salary".ToLower()).Length.Should().Be(30);
        }

        /// <summary>
        /// Convert the to data table.
        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/6ffcb247-77fb-40b4-bcba-08ba377ab9db/converting-a-list-to-datatable?forum=csharpgeneral
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The <see cref="DataTable"/>.</returns>
        /// <typeparam name="T"></typeparam>
        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        public class Person
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string Name { get; set; }
            public DateTime DateOfBirth { get; set; }
            public decimal Salary { get; set; }
        }

        public DataTable GetPeopleTest1()
        {
            return ConvertToDataTable(new List<Person> {
                new Person {
                    LastName = "a",
                    FirstName = "ab",
                    Name = "a",
                    DateOfBirth = new DateTime(1901, 1, 1),
                    Salary = 1M
                },
                new Person {
                    LastName = "abcdefghijk",
                    FirstName = "abc",
                    Name = "aa",
                    DateOfBirth = new DateTime(2001, 1, 15),
                    Salary = 10000000000M
                },
                new Person {
                    LastName = "abc",
                    FirstName = "abcdefghijklmnopqrstw",
                    Name = "aaa",
                    DateOfBirth = new DateTime(1901, 10, 15),
                    Salary = 1000000000000000000000M
                }
            });
        }
    }
}