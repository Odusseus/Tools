using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ExcelToSql.Logic.Tests
{
    [TestClass()]
    [ExcludeFromCodeCoverage]
    public class HelpTests
    {
        [TestMethod()]
        public void WriteTest_Should_Write_At_Least_One()
        {
            // Arrange
            Mock<IAssemblyLoader> mockAssemblyLoader = new Mock<IAssemblyLoader>();
            mockAssemblyLoader.Setup(m => m.GetEntryAssembly()).Returns(Assembly.LoadFrom("ExcelToSqlTests.dll"));
            IAssemblyLoader assemblyLoader = mockAssemblyLoader.Object;

            Mock<IOutputWriter> mockOutputWriter = new Mock<IOutputWriter>();
            IOutputWriter outputWriter = mockOutputWriter.Object;

            Help help = new Help(assemblyLoader, outputWriter);

            // Act
            help.Write();

            // Assert
            mockOutputWriter.Verify(m => m.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }
    }
}