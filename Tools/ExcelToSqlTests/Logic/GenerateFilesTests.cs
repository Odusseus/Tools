using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ExcelToSql.Enum;
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

        private Mock<IOutputWriter> outputWriterMock;
        private IOutputWriter outputWriter;

        [TestInitialize]
        public void TestInitialize()
        {
            this.configMock = new Mock<IConfig>();
            this.config = this.configMock.Object;

            this.spreadsheetMock = new Mock<ISpreadsheet>();
            this.spreadsheet = this.spreadsheetMock.Object;

            this.fileSystemMock = new Mock<IFileSystem>();
            this.fileSystem = this.fileSystemMock.Object;

            this.outputWriterMock = new Mock<IOutputWriter>();
            this.outputWriter = this.outputWriterMock.Object;
        }

        [TestMethod()]
        public void RunTest_Return_True()
        {
            // Arrange

            //string tabularString = JsonConvert.SerializeObject(tabular).Replace("\"","\\\"");

            string spreadsheetString = "[{\"Column0\":\"OrderDate\",\"Column1\":\"Region's\",\"Column2\":\"Rep\",\"Column3\":\"Item\",\"Column4\":\"Units\",\"Column5\":\"Unit Cost\",\"Column6\":\"Total\"},{\"Column0\":\"2016-01-06T00:00:00\",\"Column1\":\"East's\",\"Column2\":\"Jones\",\"Column3\":\"Pencil\",\"Column4\":95.0,\"Column5\":1.99,\"Column6\":189.05}]".Replace("\\", "");
            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(spreadsheetString);

            spreadsheetMock.Setup(s => s.GetTabular()).Returns(dataTable);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            // Act
            bool result = generateFiles.Run();

            // Assert
            result.Should().BeTrue();
            spreadsheetMock.Verify(s => s.GetTabular(), Times.Once);
        }

        [TestMethod()]
        public void RunTest_Return_False_When_Header()
        {
            // Arrange

            //string tabularString = JsonConvert.SerializeObject(tabular).Replace("\"","\\\"");

            //string spreadsheetString = "[{\"Column0\":\"OrderDate\",\"Column1\":\"Region's\",\"Column2\":\"Rep\",\"Column3\":\"Item\",\"Column4\":\"Units\",\"Column5\":\"Unit Cost\",\"Column6\":\"Total\"},{\"Column0\":\"2016-01-06T00:00:00\",\"Column1\":\"East's\",\"Column2\":\"Jones\",\"Column3\":\"Pencil\",\"Column4\":95.0,\"Column5\":1.99,\"Column6\":189.05}]".Replace("\\", "");
            //DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(spreadsheetString);

            spreadsheetMock.Setup(s => s.GetTabular()).Returns((DataTable)null);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            // Act
            bool result = generateFiles.Run();

            // Assert
            result.Should().BeFalse();
            spreadsheetMock.Verify(s => s.GetTabular(), Times.Once);
        }

        [TestMethod()]
        public void GetHeaderTest_Should_Return_Header_With_8_Fields()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

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
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

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

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

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

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

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

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            // Act
            generateFiles.SetFieldLength(dataTable, header);

            // Assert
            dataTable.GetHashCode().Should().Be(dataTableHash);
        }

        [TestMethod()]
        public void SetFieldLength_Should_Set_The_Length_Followed_The_Field_Length()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

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

        [TestMethod()]
        public void AddIdField_Should_Let_Header_Null_When_It_Is_Null()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = null;
            // Act
            generateFiles.AddIdField(header);

            // Assert
            header.Should().BeNull();
        }

        [TestMethod()]
        public void AddIdField_Should_Add_Id_To_The_Header()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();
            // Act
            generateFiles.AddIdField(header);

            // Assert
            header.Fields.Find(f => f.Name == "id").Length.Should().Be(Constant.Key.ID_LENGHT);
        }

        #region AddExtraFields

        [TestMethod()]
        public void AddExtraFields_Let_Header_Null_When_Is_Null()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = null;

            // Act
            generateFiles.AddExtraFields(header);

            // Assert
            header.Should().BeNull();
        }

        [TestMethod()]
        public void AddExtraFields_Should_Add_Label_Field_When_It_Is_a_OutExtraFields()
        {
            // Arrange
            string fieldName = "label";
            this.configMock.Setup(m => m.OutExtraFields).Returns(fieldName);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();

            // Act
            generateFiles.AddExtraFields(header);

            // Assert
            header.Fields.Find(f => f.Name == fieldName).Name.Should().Be(fieldName);
            header.Fields.Find(f => f.Name == fieldName).Type.Should().Be(DatabaseEnum.TypeField.Text);
            header.Fields.Find(f => f.Name == fieldName).Extra.Should().BeTrue();
        }

        [TestMethod()]
        public void AddExtraFields_Should_Add_Label_And_Message_Field_When_There_Are_OutExtraFields()
        {
            // Arrange
            string label = "label";
            string message = "message";
            string fieldName = $"{label},{message}";
            this.configMock.Setup(m => m.OutExtraFields).Returns(fieldName);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();

            // Act
            generateFiles.AddExtraFields(header);

            // Assert
            header.Fields.Find(f => f.Name == label).Name.Should().Be(label);
            header.Fields.Find(f => f.Name == message).Name.Should().Be(message);
        }

        [TestMethod()]
        public void AddExtraFields_Should_Add_Label_Field_And_Length_When_It_Is_a_OutExtraFields()
        {
            // Arrange
            string label = "label";
            int length = 100;
            string fieldName = $"{label}={length}";
            this.configMock.Setup(m => m.OutExtraFields).Returns(fieldName);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();

            // Act
            generateFiles.AddExtraFields(header);

            // Assert
            header.Fields.Find(f => f.Name == label).Length.Should().Be(length);
        }

        [TestMethod()]
        public void AddExtraFields_Should_Add_Label_And_Message_Fields_With_Lenghts_When_Where_Are_In_OutExtraFields()
        {
            // Arrange
            string label = "label";
            int length = 100;
            string message = "message";
            int lengthMessage = 200;
            string dummy = "dummy";
            string lengthDummy = "xxx";
            string fieldName = $"{label}={length},{message}={lengthMessage},{dummy}={lengthDummy}";
            this.configMock.Setup(m => m.OutExtraFields).Returns(fieldName);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();

            // Act
            generateFiles.AddExtraFields(header);

            // Assert
            header.Fields.Find(f => f.Name == label).Length.Should().Be(length);
            header.Fields.Find(f => f.Name == message).Length.Should().Be(lengthMessage);
            header.Fields.Find(f => f.Name == dummy).Length.Should().Be(10);
        }

        #endregion AddExtraFields

        #region AddExtraNumberFields

        [TestMethod()]
        public void AddExtraNumberFields_Let_Header_Null_When_Is_Null()
        {
            // Arrange
            this.configMock.Setup(m => m.OutExtraNumberFields).Returns("label");

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = null;

            // Act
            generateFiles.AddExtraNumberFields(header);

            // Assert
            header.Should().BeNull();
        }

        [TestMethod()]
        public void AddExtraNumberFields_Should_Add_Label_Field_When_It_Is_a_OutExtraNumberFields()
        {
            // Arrange
            string fieldName = "label";
            this.configMock.Setup(m => m.OutExtraNumberFields).Returns(fieldName);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();

            // Act
            generateFiles.AddExtraNumberFields(header);

            // Assert
            header.Fields.Find(f => f.Name == fieldName).Name.Should().Be(fieldName);
            header.Fields.Find(f => f.Name == fieldName).Type.Should().Be(DatabaseEnum.TypeField.Number);
            header.Fields.Find(f => f.Name == fieldName).Extra.Should().BeTrue();
        }

        [TestMethod()]
        public void AddExtraNumberFields_Should_Add_Label_And_Message_Field_When_There_Are_OutExtraNumberFields()
        {
            // Arrange
            string label = "label";
            string message = "message";
            string fieldName = $"{label},{message}";
            this.configMock.Setup(m => m.OutExtraNumberFields).Returns(fieldName);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();

            // Act
            generateFiles.AddExtraNumberFields(header);

            // Assert
            header.Fields.Find(f => f.Name == label).Name.Should().Be(label);
            header.Fields.Find(f => f.Name == message).Name.Should().Be(message);
        }

        [TestMethod()]
        public void AddExtraNumberFields_Should_Add_Label_Field_And_Length_When_It_Is_a_OutExtraNumberFields()
        {
            // Arrange
            string label = "label";
            int length = 100;
            string fieldName = $"{label}={length}";
            this.configMock.Setup(m => m.OutExtraNumberFields).Returns(fieldName);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();

            // Act
            generateFiles.AddExtraNumberFields(header);

            // Assert
            header.Fields.Find(f => f.Name == label).Length.Should().Be(length);
        }

        [TestMethod()]
        public void AddExtraNumberFields_Should_Add_Label_And_Message_Fields_With_Lenghts_When_Where_Are_In_OutExtraNumberFields()
        {
            // Arrange
            string label = "label";
            int length = 100;
            string message = "message";
            int lengthMessage = 200;
            string dummy = "dummy";
            string lengthDummy = "xxx";
            string fieldName = $"{label}={length},{message}={lengthMessage},{dummy}={lengthDummy}";
            this.configMock.Setup(m => m.OutExtraNumberFields).Returns(fieldName);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();

            // Act
            generateFiles.AddExtraNumberFields(header);

            // Assert
            header.Fields.Find(f => f.Name == label).Length.Should().Be(length);
            header.Fields.Find(f => f.Name == message).Length.Should().Be(lengthMessage);
            header.Fields.Find(f => f.Name == dummy).Length.Should().Be(10);
        }

        #endregion AddExtraNumberFields

        #region AddExtraDateFields

        [TestMethod()]
        public void AddExtraDateFields_Let_Header_Null_When_Is_Null()
        {
            // Arrange
            this.configMock.Setup(m => m.OutExtraDateFields).Returns("label");

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = null;

            // Act
            generateFiles.AddExtraDateFields(header);

            // Assert
            header.Should().BeNull();
        }

        [TestMethod()]
        public void AddExtraDateFields_Should_Add_Label_Field_When_It_Is_a_OutExtraDateFields()
        {
            // Arrange
            string fieldName = "label";
            this.configMock.Setup(m => m.OutExtraDateFields).Returns(fieldName);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();

            // Act
            generateFiles.AddExtraDateFields(header);

            // Assert
            header.Fields.Find(f => f.Name == fieldName).Name.Should().Be(fieldName);
            header.Fields.Find(f => f.Name == fieldName).Type.Should().Be(DatabaseEnum.TypeField.Date);
            header.Fields.Find(f => f.Name == fieldName).Extra.Should().BeTrue();
        }

        [TestMethod()]
        public void AddExtraDateFields_Should_Add_Label_And_Message_Field_When_There_Are_OutExtraDateFields()
        {
            // Arrange
            string label = "label";
            string message = "message";
            string fieldName = $"{label},{message}";
            this.configMock.Setup(m => m.OutExtraDateFields).Returns(fieldName);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();

            // Act
            generateFiles.AddExtraDateFields(header);

            // Assert
            header.Fields.Find(f => f.Name == label).Name.Should().Be(label);
            header.Fields.Find(f => f.Name == message).Name.Should().Be(message);
        }

        [TestMethod()]
        public void AddExtraDateFields_Should_Add_Label_Field_And_Length_When_It_Is_a_OutExtraDateFields()
        {
            // Arrange
            string label = "label";
            string fieldName = $"{label}";
            this.configMock.Setup(m => m.OutExtraDateFields).Returns(fieldName);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();

            // Act
            generateFiles.AddExtraDateFields(header);

            // Assert
            header.Fields.Find(f => f.Name == label).Length.Should().Be(20);
        }

        [TestMethod()]
        public void AddExtraDateFields_Should_Add_Label_And_Message_Fields_With_Lenghts_When_Where_Are_In_OutExtraDateFields()
        {
            // Arrange
            string label = "label";
            string message = "messageextralongbighuge";
            string fieldName = $"{label},{message}";
            this.configMock.Setup(m => m.OutExtraDateFields).Returns(fieldName);

            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();

            // Act
            generateFiles.AddExtraDateFields(header);

            // Assert
            header.Fields.Find(f => f.Name == label).Length.Should().Be(20);
            header.Fields.Find(f => f.Name == message).Length.Should().Be(30);
        }

        #endregion AddExtraDateFields

        #region CreateSqlScript

        [TestMethod()]
        public void CreateSqlScript_Return_False_When_Header_Is_Null()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = null;

            // Act
            bool result = generateFiles.CreateSqlScript(header);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod()]
        public void CreateSqlScript_Return_Postgres_Script_When_Vendor_Is_Postpres()
        {
            // Arrange
            this.configMock.Setup(c => c.DatabaseVendor).Returns(DatabaseEnum.Vendor.Postgres);
            this.configMock.Setup(c => c.OutTablename).Returns("person");
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            List<string> lines = new List<string>();

            this.fileSystemMock.Setup(o => o.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Encoding>())).Callback((string path, List<string> contents, Encoding encoding) => lines = contents);

            Header header = new Header
            {
                Fields = new List<Field>
                {
                    new Field
                    {
                        Column = 0,
                        Extra = false,
                        Length = 10,
                        Row = 0,
                        Text = "Name",
                        Type = DatabaseEnum.TypeField.Text
                    },
                    new Field
                    {
                        Column = 1,
                        Extra = false,
                        Length = 20,
                        Row = 0,
                        Text = "BirthDay",
                        Type = DatabaseEnum.TypeField.Date
                    },
                    new Field
                    {
                        Column = 2,
                        Extra = true,
                        Length = 9,
                        Row = 0,
                        Text = "Id",
                        Type = DatabaseEnum.TypeField.Number
                    }
                }
            };

            // Act
            bool result = generateFiles.CreateSqlScript(header);

            // Assert
            result.Should().BeTrue();
            lines.Count.Should().BeGreaterThan(0);
            lines[0].Should().Be("CREATE TABLE person (");
            lines[1].Should().Be("name VARCHAR(10),");
            lines[2].Should().Be("birthday DATE,");
            lines[3].Should().Be("id BIGINT);");
            lines[4].Should().Be(string.Empty);
            lines[5].Should().Be("CREATE UNIQUE INDEX person_pk_index ON person (id);");
        }

        [TestMethod()]
        public void CreateSqlScript_Return_Oracle_Script_When_Vendor_Is_Postpres()
        {
            // Arrange
            this.configMock.Setup(c => c.DatabaseVendor).Returns(DatabaseEnum.Vendor.Oracle);
            this.configMock.Setup(c => c.OutTablename).Returns("person");
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            List<string> lines = new List<string>();

            this.fileSystemMock.Setup(o => o.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Encoding>())).Callback((string path, List<string> contents, Encoding encoding) => lines = contents);

            Header header = new Header
            {
                Fields = new List<Field>
                {
                    new Field
                    {
                        Column = 0,
                        Extra = false,
                        Length = 10,
                        Row = 0,
                        Text = "Name",
                        Type = DatabaseEnum.TypeField.Text
                    },
                    new Field
                    {
                        Column = 1,
                        Extra = false,
                        Length = 20,
                        Row = 0,
                        Text = "BirthDay",
                        Type = DatabaseEnum.TypeField.Date
                    },
                    new Field
                    {
                        Column = 2,
                        Extra = true,
                        Length = 9,
                        Row = 0,
                        Text = "Id",
                        Type = DatabaseEnum.TypeField.Number
                    }
                }
            };

            // Act
            bool result = generateFiles.CreateSqlScript(header);

            // Assert
            result.Should().BeTrue();
            lines.Count.Should().BeGreaterThan(0);
            lines[0].Should().Be("CREATE TABLE person (");
            lines[1].Should().Be("name VARCHAR2(10),");
            lines[2].Should().Be("birthday DATE,");
            lines[3].Should().Be("id NUMBER(9));");
            lines[4].Should().Be(string.Empty);
            lines[5].Should().Be("CREATE UNIQUE INDEX person_pk_index ON person (id);");
        }

        #endregion CreateSqlScript

        #region InsertSqlScript

        [TestMethod()]
        public void InsertSqlScript_Should_Return_0_When_Header_Is_Null()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = null;
            DataTable dataTable = new DataTable();

            // Act
            int result = generateFiles.InsertSqlScript(header, dataTable);

            // Assert
            result.Should().Be(0);
        }

        [TestMethod()]
        public void InsertSqlScript_Should_Return_0_When_HeaderFileds_Is_Null()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();
            DataTable dataTable = new DataTable();

            // Act
            int result = generateFiles.InsertSqlScript(header, dataTable);

            // Assert
            result.Should().Be(0);
        }

        [TestMethod()]
        public void InsertSqlScript_Should_Return_0_When_DataTable_Is_Null()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header
            {
                Fields = new List<Field>()
            };
            DataTable dataTable = null;

            // Act
            int result = generateFiles.InsertSqlScript(header, dataTable);

            // Assert
            result.Should().Be(0);
        }

        [TestMethod()]
        public void InsertSqlScript_Should_Return_0_When_DataTableRows_Is_Null()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header
            {
                Fields = new List<Field>()
            };
            DataTable dataTable = new DataTable();

            // Act
            int result = generateFiles.InsertSqlScript(header, dataTable);

            // Assert
            result.Should().Be(0);
        }

        [TestMethod()]
        public void InsertSqlScript_Should_Return_0_When_Header_And_DataTable_Are_Null()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = null;
            DataTable dataTable = null;

            // Act
            int result = generateFiles.InsertSqlScript(header, dataTable);

            // Assert
            result.Should().Be(0);
        }

        [TestMethod()]
        public void InsertSqlScript_Should_Return_0_When_HeaderFileds_And_DataTableRows_Are_Null()
        {
            // Arrange
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            Header header = new Header();
            DataTable dataTable = new DataTable();

            // Act
            int result = generateFiles.InsertSqlScript(header, dataTable);

            // Assert
            result.Should().Be(0);
        }

        [TestMethod()]
        public void InsertSqlScript_Should_Return_Postgres_Insert_Script()
        {
            // Arrange
            this.configMock.Setup(c => c.DatabaseVendor).Returns(DatabaseEnum.Vendor.Postgres);
            this.configMock.Setup(c => c.OutTablename).Returns("person");
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            List<string> lines = new List<string>();

            this.fileSystemMock.Setup(o => o.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Encoding>())).Callback((string path, List<string> contents, Encoding encoding) => lines = contents);

            DataTable dataTable = GetPeopleTest1();

            Header header = new Header
            {
                Fields = new List<Field>
                {
                    new Field
                    {
                        Column = 0,
                        Text = "LastName",
                        Length = 10,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Text
                    },
                    new Field
                    {
                        Column = 1,
                        Text = "FirstName",
                        Length = 20,
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
                        Length = 20,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Date
                    },
                    new Field
                    {
                        Column = 4,
                        Text = "Salary",
                        Length = 30,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Number
                    },
                    new Field
                    {
                        Column = 5,
                        Extra = false,
                        Text = "id",
                        Length = 9,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Number
                    },
                    new Field
                    {
                        Column = 6,
                        Extra = true,
                        Text = "ExternalNumber",
                        Length = 9,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Number
                    },
                    new Field
                    {
                        Column = 7,
                        Extra = true,
                        Text = "Message",
                        Length = 2000,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Text
                    }
                }
            };

            // Act
            int result = generateFiles.InsertSqlScript(header, dataTable);

            // Assert
            result.Should().Be(2);
            lines.Count.Should().BeGreaterThan(0);
            lines[0].Should().Be("INSERT INTO person (lastname, firstname, name, dateofbirth, salary, id, externalnumber, message) VALUES ('abcdefghijk', 'abc', 'aa', '15-1-2001', '10000000000', 1, null, null);");
            lines[1].Should().Be("INSERT INTO person (lastname, firstname, name, dateofbirth, salary, id, externalnumber, message) VALUES ('abc', 'abcdefghijklmnopqrstw', 'aaa', '15-10-1901', '1000000000000000000000', 2, null, null);");
        }

        [TestMethod()]
        public void InsertSqlScript_Should_Return_Oracle_Insert_Script()
        {
            // Arrange
            this.configMock.Setup(c => c.DatabaseVendor).Returns(DatabaseEnum.Vendor.Oracle);
            this.configMock.Setup(c => c.OutTablename).Returns("person");
            GenerateFiles generateFiles = new GenerateFiles(this.config, this.spreadsheet, this.fileSystem, this.outputWriter);

            List<string> lines = new List<string>();

            this.fileSystemMock.Setup(o => o.WriteAllLines(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Encoding>())).Callback((string path, List<string> contents, Encoding encoding) => lines = contents);

            DataTable dataTable = GetPeopleTest1();

            Header header = new Header
            {
                Fields = new List<Field>
                {
                    new Field
                    {
                        Column = 0,
                        Text = "LastName",
                        Length = 10,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Text
                    },
                    new Field
                    {
                        Column = 1,
                        Text = "FirstName",
                        Length = 20,
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
                        Length = 20,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Date
                    },
                    new Field
                    {
                        Column = 4,
                        Text = "Salary",
                        Length = 30,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Number
                    },
                    new Field
                    {
                        Column = 5,
                        Extra = false,
                        Text = "id",
                        Length = 9,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Number
                    },
                    new Field
                    {
                        Column = 6,
                        Extra = true,
                        Text = "ExternalNumber",
                        Length = 9,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Number
                    },
                    new Field
                    {
                        Column = 7,
                        Extra = true,
                        Text = "Message",
                        Length = 2000,
                        Row = 0,
                        Type = Enum.DatabaseEnum.TypeField.Text
                    }
                }
            };

            // Act
            int result = generateFiles.InsertSqlScript(header, dataTable);

            // Assert
            result.Should().Be(2);
            lines.Count.Should().BeGreaterThan(0);
            lines[0].Should().Be("INSERT INTO person (lastname, firstname, name, dateofbirth, salary, id, externalnumber, message) VALUES ('abcdefghijk', 'abc', 'aa', '15-1-2001', '10000000000', 1, null, null);");
            lines[1].Should().Be("INSERT INTO person (lastname, firstname, name, dateofbirth, salary, id, externalnumber, message) VALUES ('abc', 'abcdefghijklmnopqrstw', 'aaa', '15-10-1901', '1000000000000000000000', 2, null, null);");
            lines[2].Should().Be("");
            lines[3].Should().Be($"{Constant.Key.COMMIT};");
        }

        #endregion InsertSqlScript

        #region Private

        /// <summary>
        /// Convert the to data table.
        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/6ffcb247-77fb-40b4-bcba-08ba377ab9db/converting-a-list-to-datatable?forum=csharpgeneral
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The <see cref="DataTable"/>.</returns>
        /// <typeparam name="T"></typeparam>
        private DataTable ConvertToDataTable<T>(IList<T> data)
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

        private class Person
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string Name { get; set; }
            public DateTime DateOfBirth { get; set; }
            public decimal Salary { get; set; }
        }

        private DataTable GetPeopleTest1()
        {
            return ConvertToDataTable(new List<Person> {
                new Person { // first would be skiped because normaly it is a header
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

        #endregion Private
    }
}