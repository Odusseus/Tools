using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExcelToSql.Logic.Tests
{
    [TestClass()]
    public class ExtensionMethodsTests
    {
        [TestMethod()]
        public void RoundUp_Should_Return_10_When_Values_Is_0()
        {
            // Arrange
            int value = 0;

            // Act
            var result = value.RoundUp();

            // Assert
            result.Should().Be(10);

        }

        [TestMethod()]
        public void RoundUp_Should_Return_10_When_Values_Is_7()
        {
            // Arrange
            int value = 7;

            // Act
            var result = value.RoundUp();

            // Assert
            result.Should().Be(10);

        }

        [TestMethod()]
        public void RoundUp_Should_Return_10_When_Values_Is_Negatief()
        {
            // Arrange
            int value = -10;

            // Act
            var result = value.RoundUp();

            // Assert
            result.Should().Be(10);

        }
        [TestMethod()]
        public void RoundUp_Should_Return_20_When_Values_Is_10()
        {
            // Arrange
            int value = 10;

            // Act
            var result = value.RoundUp();

            // Assert
            result.Should().Be(20);

        }
        
        #region Clean
        [TestMethod()]
        [DataRow("abc", "abc")]
        [DataRow("ABC", "abc")]
        [DataRow("   ABC", "abc")]
        [DataRow("ABC   ", "abc")]
        [DataRow("   ABC   ", "abc")]
        [DataRow("A B C", "a_b_c")]
        [DataRow("äûç", "auc")]

        public void CleanTest(string text, string assertText)
        {
            // Arrange
            
            // Act
            string result = text.Clean();

            // Assert
            result.Should().Be(assertText);
        }
        #endregion Clean
    }
}