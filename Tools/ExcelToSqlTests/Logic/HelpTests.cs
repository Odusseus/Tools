using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AutoFixture;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ExcelToSql.Logic.Tests
{
    [TestClass()]
    [ExcludeFromCodeCoverage]
    public class HelpTests
    {
        [TestMethod()]
        public void WriteTest_Should_Write_At_Least_One_And_Call_Exit_With_Level_minOne()
        {
            // Arrange
            var fixture = new Fixture();

            Mock<IApp> mockApp = new Mock<IApp>();
            IApp app = mockApp.Object;

            Mock<IAssemblyLoader> mockAssemblyLoader = new Mock<IAssemblyLoader>();
            mockAssemblyLoader.Setup(m => m.GetEntryAssembly()).Returns(Assembly.LoadFrom("ExcelToSqlTests.dll"));
            IAssemblyLoader assemblyLoader = mockAssemblyLoader.Object;

            Mock<IOutputWriter> mockOutputWriter = new Mock<IOutputWriter>();
            IOutputWriter outputWriter = mockOutputWriter.Object;

            Help help = new Help(assemblyLoader, app, outputWriter);

            // Act
            help.Write();

            // Assert
            mockApp.Verify(m => m.Exit(-1), Times.Once());
            mockOutputWriter.Verify(m => m.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }
    }
}