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
        [TestMethod()]
        public void RunTest_Return_True()
        {
            // Arrange
            Mock<IConfig> configMock = new Mock<IConfig>();
            IConfig config = configMock.Object;

            Mock<ISpreadsheet> spreadsheetMock = new Mock<ISpreadsheet>();
            ISpreadsheet spreadsheet = spreadsheetMock.Object;

            //string tabularString = JsonConvert.SerializeObject(tabular).Replace("\"","\\\"");

            string spreadsheetString = "[{\"Column0\":\"OrderDate\",\"Column1\":\"Region's\",\"Column2\":\"Rep\",\"Column3\":\"Item\",\"Column4\":\"Units\",\"Column5\":\"Unit Cost\",\"Column6\":\"Total\"},{\"Column0\":\"2016-01-06T00:00:00\",\"Column1\":\"East's\",\"Column2\":\"Jones\",\"Column3\":\"Pencil\",\"Column4\":95.0,\"Column5\":1.99,\"Column6\":189.05}]".Replace("\\", "");
            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(spreadsheetString);

            spreadsheetMock.Setup(s => s.GetTabular()).Returns(dataTable);

            Mock<IFileSystem> fileSystemMock = new Mock<IFileSystem>();
            IFileSystem fileSystem = fileSystemMock.Object;

            GenerateFiles generateFiles = new GenerateFiles(config, spreadsheet, fileSystem);

            // Act
            bool result = generateFiles.Run();

            // Assert
            result.Should().BeTrue();
            spreadsheetMock.Verify(s => s.GetTabular(), Times.Once);
        }
    }
}