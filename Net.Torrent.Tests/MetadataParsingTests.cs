using Net.Torrent.BEncode;
using System;
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
            var contens = GetTorrentContents();
            var parser = new TorrentSerializer();
            var trt = parser.Deserialize(new ReadOnlySpan<byte>(contens));
        }

        
        [Fact]
        public void ReturnsComment()
        {
            var contens = GetTorrentContents();
            var parser = new TorrentSerializer();
            var trt = parser.Deserialize(new ReadOnlySpan<byte>(contens));
            Assert.True(trt.TryGetExtension(ExtensionKeys.Comment, out BString value));
            Assert.NotNull(value);
        }

        
        [Fact]
        public void ReturnsPublisherUri()
        {
            var contens = GetTorrentContents();
            var parser = new TorrentSerializer();
            var trt = parser.Deserialize(new ReadOnlySpan<byte>(contens));
            Assert.True(trt.TryGetExtension(ExtensionKeys.PublisherUrl, out BString value));
            Assert.NotNull(value);
        }

        [Fact]
        public void ReturnsPublisher()
        {
            var contens = GetTorrentContents();
            var parser = new TorrentSerializer();
            var trt = parser.Deserialize(new ReadOnlySpan<byte>(contens));
            Assert.True(trt.TryGetExtension(ExtensionKeys.Publisher, out BString value));
            Assert.NotNull(value);
        }

        [Fact]
        public void ReturnsCreationDate()
        {
            var contens = GetTorrentContents();
            var parser = new TorrentSerializer();
            var trt = parser.Deserialize(new ReadOnlySpan<byte>(contens));
            Assert.True(trt.TryGetExtension(ExtensionKeys.CreationDate, out BNumber value));
            Assert.NotNull(value);
        }

        [Fact]
        public void ReturnsCreator()
        {
            var contens = GetTorrentContents();
            var parser = new TorrentSerializer();
            var trt = parser.Deserialize(new ReadOnlySpan<byte>(contens));
            Assert.True(trt.TryGetExtension(ExtensionKeys.CreatedBy, out BString value));
            Assert.NotNull(value);
        }

        [Fact]
        public void SavesFile()
        {
            var contens = GetTorrentContents();
            var parser = new TorrentSerializer();
            var trt = parser.Deserialize(new ReadOnlySpan<byte>(contens));
            var builder = TorrentBuilder.FromExisting(trt);
            builder.SetName("русский беларускі ў");
            var modified = builder.Build();
            using(var file = File.Create("output.torrent"))
            {
                parser.Serialize(file, modified);
            }
        }

        private byte[] GetTorrentContents()
        {
            using (var file = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}
