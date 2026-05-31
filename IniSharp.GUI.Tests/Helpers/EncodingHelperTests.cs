using System.Text;
using IniSharp.GUI;

namespace IniSharp.GUI.Tests.Helpers
{
    [TestFixture]
    public class EncodingHelperTests
    {
        private string _testDir = null!;

        [SetUp]
        public void SetUp()
        {
            _testDir = Path.Combine(Path.GetTempPath(), $"EncodingHelperTests_{Guid.NewGuid():N}");
            Directory.CreateDirectory(_testDir);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_testDir))
            {
                Directory.Delete(_testDir, recursive: true);
            }
        }

        [Test]
        public void DetectEncoding_WithUtf32LittleEndianBom_ReturnsUtf32()
        {
            // Arrange
            var filePath = Path.Combine(_testDir, "utf32le.ini");
            File.WriteAllText(filePath, "[Section]\nkey=value", Encoding.UTF32);

            // Act
            var encoding = EncodingHelper.DetectEncoding(filePath);

            // Assert
            Assert.That(encoding.CodePage, Is.EqualTo(Encoding.UTF32.CodePage));
        }

        [Test]
        public void DetectEncoding_WithUtf32BigEndianBom_ReturnsUtf32BigEndian()
        {
            // Arrange
            var filePath = Path.Combine(_testDir, "utf32be.ini");
            var utf32BigEndian = new UTF32Encoding(bigEndian: true, byteOrderMark: true);
            File.WriteAllText(filePath, "[Section]\nkey=value", utf32BigEndian);

            // Act
            var encoding = EncodingHelper.DetectEncoding(filePath);

            // Assert
            Assert.That(encoding.CodePage, Is.EqualTo(utf32BigEndian.CodePage));
        }
    }
}
