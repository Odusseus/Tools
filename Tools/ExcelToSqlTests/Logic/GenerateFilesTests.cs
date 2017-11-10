using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ExcelToSql.Logic.Tests
{
    [TestClass()]
    public class GenerateFilesTests
    {
        [TestMethod()]
        public void RunTest_Return_True()
        {
            // Arrange
            Mock<IConfig> IConfigMock = new Mock<IConfig>();
            IConfig config = IConfigMock.Object;

            Mock<IFileSystem> IFileSystemMock = new Mock<IFileSystem>();
            IFileSystem fileSystem  = IFileSystemMock.Object;

            GenerateFiles generateFiles = new GenerateFiles(config, fileSystem);

            // Act
            //bool result = generateFiles.Run();

            // Assert
            // TODO
            //result.Should().BeTrue();
        }
    }
}