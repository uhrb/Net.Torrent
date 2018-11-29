using Net.Torrent.BEncode;
using System.IO;
using Xunit;

namespace Net.Torrent.Test
{
    public class MetadataParsingTests
    {
        private const string FileName = "invincible.torrent";
        [Fact]
        public void RealTorrentParsed()
        {
            using (var file = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var parser = new TorrentSerializer();
                var trt = parser.Deserialize(file);
            }
        }


        [Fact]
        public void ReturnsComment()
        {
            using (var file = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var parser = new TorrentSerializer();
                var trt = parser.Deserialize(file);
                Assert.True(trt.TryGetExtension(ExtensionKeys.Comment, out BString value));
                Assert.NotNull(value);
            }
        }


        [Fact]
        public void ReturnsPublisherUri()
        {
            using (var file = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var parser = new TorrentSerializer();
                var trt = parser.Deserialize(file);
                Assert.True(trt.TryGetExtension(ExtensionKeys.PublisherUrl, out BString value));
                Assert.NotNull(value);
            }
        }

        [Fact]
        public void ReturnsPublisher()
        {
            using (var file = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var parser = new TorrentSerializer();
                var trt = parser.Deserialize(file);
                Assert.True(trt.TryGetExtension(ExtensionKeys.Publisher, out BString value));
                Assert.NotNull(value);
            }
        }

        [Fact]
        public void ReturnsCreationDate()
        {
            using (var file = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var parser = new TorrentSerializer();
                var trt = parser.Deserialize(file);
                Assert.True(trt.TryGetExtension(ExtensionKeys.CreationDate, out BNumber value));
                Assert.NotNull(value);
            }
        }

        [Fact]
        public void ReturnsCreator()
        {
            using (var file = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var parser = new TorrentSerializer();
                var trt = parser.Deserialize(file);
                Assert.True(trt.TryGetExtension(ExtensionKeys.CreatedBy, out BString value));
                Assert.NotNull(value);
            }
        }

        [Fact]
        public void SavesFile()
        {
            using (var file = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var parser = new TorrentSerializer();
                var trt = parser.Deserialize(file);
                var builder = TorrentBuilder.FromExisting(trt);
                builder.SetName("русский беларускі ў");
                var modified = builder.Build();
                using (var file2 = File.Create("output.torrent"))
                {
                    parser.Serialize(file2, modified);
                }
            }
        }
    }
}
