using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using ExcelToSql.Constant;
using ExcelToSql.Enum;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExcelToSql.Logic.Tests
{
    [ExcludeFromCodeCoverage]
    public class ConfigTests
    {
        [Fact]
        public void ConfigTest_Should_Return_DatabaseVendor_Oracle_By_Default()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;

            Fixture fixture = new Fixture();
            var databaseVendor = fixture.Create("vendor");

            configurationManagerLoaderMock.Setup(c => c.KeyDatabaseVendor).Returns(databaseVendor);

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.DatabaseVendor.Should().Be(DatabaseEnum.Vendor.Oracle);
        }

        [Fact]
        public void ConfigTest_Should_Return_DatabaseVendor_Oracle_When_DatabaseVendor_Is_Oracle()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyDatabaseVendor).Returns("ABC");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.DatabaseVendor.Should().Be(DatabaseEnum.Vendor.Oracle);
        }

        [Fact]
        public void ConfigTest_Should_Return_DatabaseVendor_Postgres_When_DatabaseVendor_Is_Postgres()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyDatabaseVendor).Returns("Postgres");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.DatabaseVendor.Should().Be(DatabaseEnum.Vendor.Postgres);
        }

        [Fact]
        public void ConfigTest_Should_Return_ExcelFilename_When_This_Is_abc()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyExcelFilename).Returns("abc");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.ExcelFilename.Should().Be("abc");
        }

        [Fact]
        public void ConfigTest_Should_Return_ExcelPath_When_This_Is_abc()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyExcelPath).Returns("abc");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.ExcelPath.Should().Be("abc");
        }

        [Fact]
        public void ConfigTest_Should_Return_ExcelTabular_When_This_Is_abc()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyExcelTabular).Returns("abc");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.ExcelTabular.Should().Be("abc");
        }

        [Fact]
        public void ConfigTest_Should_Return_OutCreateFilename_When_This_Is_abc()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyOutCreateFilename).Returns("abc");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.OutCreateFilename.Should().Be("abc");
        }

        [Fact]
        public void ConfigTest_Should_Return_OutExtraDateFields_When_This_Is_abc()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyOutExtraDateFields).Returns("abc");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.OutExtraDateFields.Should().Be("abc");
        }

        [Fact]
        public void ConfigTest_Should_Return_OutExtraFields_When_This_Is_abc()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyOutExtraFields).Returns("abc");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.OutExtraFields.Should().Be("abc");
        }

        [Fact]
        public void ConfigTest_Should_Return_OutExtraNumberFields_When_This_Is_abc()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyOutExtraNumberFields).Returns("abc");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.OutExtraNumberFields.Should().Be("abc");
        }

        [Fact]
        public void ConfigTest_Should_Return_OutFileEncoding_123_When_This_Is_123()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyOutFileEncoding).Returns("123");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.OutFileEncoding.Should().Be(123);
        }

        [Fact]
        public void ConfigTest_Should_Return_OutFileEncoding_Utf8_By_Default()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyOutFileEncoding).Returns("");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.OutFileEncoding.Should().Be(Key.Utf8);
        }

        [Fact]
        public void ConfigTest_Should_Return_OutInsertFilename_When_This_Is_abc()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyOutInsertFilename).Returns("abc");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.OutInsertFilename.Should().Be("abc");
        }

        [Fact]
        public void ConfigTest_Should_Return_OutPath_When_This_Is_abc()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyOutPath).Returns("abc");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.OutPath.Should().Be("abc");
        }

        [Fact]
        public void ConfigTest_Should_Return_OutStartId_0_By_Default()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyOutStartId).Returns("abc");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.OutStartId.Should().Be(0);
        }

        [Fact]
        public void ConfigTest_Should_Return_OutStartId_123_When_This_Is_123()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyOutStartId).Returns("123");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.OutStartId.Should().Be(123);
        }

        [Fact]
        public void ConfigTest_Should_Return_OutTablename_When_This_Is_abc()
        {
            // Arrange
            Mock<IConfigurationManagerLoader> configurationManagerLoaderMock = new Mock<IConfigurationManagerLoader>();
            IConfigurationManagerLoader configurationManagerLoader = configurationManagerLoaderMock.Object;
            configurationManagerLoaderMock.Setup(c => c.KeyOutTablename).Returns("abc");

            // Act
            Config config = new Config(configurationManagerLoader);

            // Assert
            config.OutTablename.Should().Be("abc");
        }

        [Fact]
        public void ConfigTest()
        {
            // Act
            var config = Config.Instance;

            // Assert
            config.Should().NotBeNull();
        }
    }
}