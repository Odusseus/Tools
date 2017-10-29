using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelToSql.Enum;
using ExcelToSql.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace ExcelToSqlTests.Logic
{
    [TestClass]
    public class FieldTests
    {
        [TestMethod]
        public void Field_Object_Is_Correct_Filled()
        {
            // Arrange
            int column = 1;
            bool extra = true;
            int length = 20;
            int row = 2;
            string text = "Dummytext";
            string nameAssert = "dummytext";
            DatabaseEnum.TypeField type = DatabaseEnum.TypeField.Text;

            // Act
            Field field = new Field
            {
                Column = column,
                Extra = extra,
                Length = length,
                Row = row,
                Text = text,
                Type = type
            };

            // Assert
            field.Column.Should().Be(column);
            field.Extra.Should().Be(extra);
            field.Length.Should().Be(length);
            field.Name.Should().Be(nameAssert);
            field.Row.Should().Be(row);
            field.Text.Should().Be(text);
            field.Type.Should().Be(type);
        }

        [TestMethod]
        public void Field_Name_Is_Trimed()
        {
            // Arrange
            string text = "    dummytext     ";
            string nameAssert = "dummytext";

            // Act
            Field field = new Field
            {
                Text = text,
            };

            // Assert
            field.Name.Should().Be(nameAssert);
        }

        [TestMethod]
        public void Field_Name_Quotes_Ares_Replaced_With_Underscores()
        {
            // Arrange
            string text = "'dummy'text'";
            string nameAssert = "_dummy_text_";

            // Act
            Field field = new Field
            {
                Text = text,
            };

            // Assert
            field.Name.Should().Be(nameAssert);
        }

        [TestMethod]
        public void Field_Name_Space_Ares_Replaced_With_Underscores()
        {
            // Arrange
            string text = "du mmy text ";
            string nameAssert = "du_mmy_text";

            // Act
            Field field = new Field
            {
                Text = text,
            };

            // Assert
            field.Name.Should().Be(nameAssert);
        }

    }
}
