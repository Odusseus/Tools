using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace ExcelToSql.Logic.Tests
{
    [ExcludeFromCodeCoverage]
    public class ExtensionMethodsTests
    {
        [Theory]
        [InlineData("abc", "abc")]
        [InlineData("ABC", "abc")]
        [InlineData("   ABC", "abc")]
        [InlineData("ABC   ", "abc")]
        [InlineData("   ABC   ", "abc")]
        [InlineData("A B C", "a_b_c")]
        [InlineData("äûç", "auc")]
        [InlineData("!'a'b|c", "__a_b_c")]
        public void CleanTest(string text, string assertText)
        {
            // Arrange

            // Act
            string result = text.Clean();

            // Assert
            result.Should().Be(assertText);
        }

        [Theory]
        [InlineData(-20, 10)]
        [InlineData(-10, 10)]
        [InlineData(-5, 10)]
        [InlineData(0, 10)]
        [InlineData(5, 10)]
        [InlineData(10, 20)]
        [InlineData(15, 20)]
        [InlineData(20, 30)]
        public void RoundUpTest(int number, int assert)
        {
            // Act
            int result = number.RoundUp();

            // Assert
            result.Should().Be(assert);
        }

        [Theory]
        [InlineData("äéû", "aeu")]
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