using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExcelToSql.Tests
{
    [TestClass()]
    [ExcludeFromCodeCoverage]
    public class ExcelToSqlTests
    {
        #region GetHelp

        [TestMethod()]
        public void GetHelp_Without_Arguments_Should_Retrun_True()
        {
            string[] args = { };
            bool result = ExcelToSql.GetHelp(args);

            result.Should().BeTrue();
        }

        [TestMethod()]
        public void GetHelp_With_Arguments_Dummy_Should_Retrun_True()
        {
            string[] args = { "Dummy" };
            bool result = ExcelToSql.GetHelp(args);

            result.Should().BeTrue();
        }

        [TestMethod()]
        public void GetHelp_With_Arguments_GO_Should_Retrun_False()
        {
            string[] args = { "GO" };
            bool result = ExcelToSql.GetHelp(args);

            result.Should().BeFalse();
        }

        [TestMethod()]
        public void GetHelp_With_Arguments_go_Should_Retrun_False()
        {
            string[] args = { "go" };
            bool result = ExcelToSql.GetHelp(args);

            result.Should().BeFalse();
        }

        [TestMethod()]
        public void GetHelp_With_Argument_Null_Should_Retrun_True()
        {
            string[] args = { null };
            bool result = ExcelToSql.GetHelp(args);

            result.Should().BeTrue();
        }

        #endregion GetHelp
    }
}