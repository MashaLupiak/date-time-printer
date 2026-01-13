using DateTimePrinter;
using System.Text.Json;

namespace UnitTests
{
    public class ConfigLoaderTests 
    {
        private static string CreateTempFile(string content)
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
            File.WriteAllText(path, content);
            return path;
        }

        [Fact]
        public void LoadConfig_WhenJsonIsValid_ShouldReturnConfig()
        {
            // Arrange
            var path = CreateTempFile("""
                {
                "DateFormat": "yyyy",
                "IntervalInSeconds": 2,
                "Message": "Hi"
                }
                """);
            var loader = new ConfigLoader(path);

            // Act
            var config = loader.LoadConfig();

            // Assert
            Assert.NotNull(config);
            Assert.Equal("yyyy", config.DateFormat);
            Assert.Equal(2, config.IntervalInSeconds);
            Assert.Equal("Hi", config.Message);
        }

        [Fact]
        public void LoadConfig_WhenIntervalIsZeroOrNegative_ShouldThrowException()
        {
            // Arrange
            var path = CreateTempFile("""
                {
                "DateFormat": "yyyy-MM-dd",
                "IntervalInSeconds": 0,
                "Message": "Hi"
                }
             """);
            var loader = new ConfigLoader(path);

            // Act
            var ex = Assert.Throws<Exception>(() => loader.LoadConfig());

            // Assert
            Assert.Equal("Failed to read config file: Interval must be > 0", ex.Message);
            Assert.Null(ex.InnerException);
        }

        [Fact]
        public void LoadConfig_WhenDateFormatIsMissingOrWhitespace_ShouldSetDefaultDateFormat()
        {
            // Arrange
            var path = CreateTempFile("""
                {
                "IntervalInSeconds": 5,
                "Message": "Hi"
                }
             """);
            var loader = new ConfigLoader(path);

            // Act
            var config = loader.LoadConfig();

            // Assert
            Assert.Equal("yyyy-MM-dd HH:mm:ss", config.DateFormat);
            Assert.Equal(5, config.IntervalInSeconds);
        }

        [Fact]
        public void LoadConfig_WhenJsonIsInvalid_ShouldThrowInvalidJsonException()
        {
            // Arrange
            var path = CreateTempFile("{ invalid json");
            var loader = new ConfigLoader(path);

            // Act
            var ex = Assert.Throws<Exception>(() => loader.LoadConfig());

            // Assert
            Assert.Equal("Config file contains invalid JSON", ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.IsType<JsonException>(ex.InnerException);
        }

        [Fact]
        public void LoadConfig_WhenFileIsLocked_ShouldThrowCannotBeReadException()
        {
            // Arrange
            var path = CreateTempFile("""
                {
                "DateFormat": "yyyy", 
                "IntervalInSeconds": 2,
                "Message": "Hi"
                }
             """);
            var loader = new ConfigLoader(path);

            using var lockStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

            // Act
            var ex = Assert.Throws<Exception>(() => loader.LoadConfig());

            // Assert
            Assert.Equal("Config file cannot be read (it may be in use)", ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.IsType<IOException>(ex.InnerException);
        }

    }
}