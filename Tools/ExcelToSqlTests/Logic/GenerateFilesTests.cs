using ExcelToSql.Logic;
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
    }
}