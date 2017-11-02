using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExcelToSql.Logic.Tests
{
    [TestClass()]
    [ExcludeFromCodeCoverage]
    public class ExtensionMethodsTests
    {
        [TestMethod()]
        [DataRow("abc", "abc")]
        [DataRow("ABC", "abc")]
        [DataRow("   ABC", "abc")]
        [DataRow("ABC   ", "abc")]
        [DataRow("   ABC   ", "abc")]
        [DataRow("A B C", "a_b_c")]
        [DataRow("äûç", "auc")]
        [DataRow("!'a'b|c", "__a_b_c")]
        public void CleanTest(string text, string assertText)
        {
            // Arrange

            // Act
            string result = text.Clean();

            // Assert
            result.Should().Be(assertText);
        }

        [TestMethod()]
        [DataRow(-20, 10)]
        [DataRow(-10, 10)]
        [DataRow(-5, 10)]
        [DataRow(0, 10)]
        [DataRow(5, 10)]
        [DataRow(10, 20)]
        [DataRow(15, 20)]
        [DataRow(20, 30)]
        public void RoundUpTest(int number, int assert)
        {
            // Act
            int result = number.RoundUp();

            // Assert
            result.Should().Be(assert);
        }

        [DataRow("äéû", "aeu")]
        public void RemoveDiacriticsTest(string text, string assertText)
        {
            // Arrange

            // Act
            string result = text.RemoveDiacritics();

            // Assert
            result.Should().Be(assertText);
        }
    }
}