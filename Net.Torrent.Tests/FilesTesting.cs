using Net.Torrent.BEncode;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Net.Torrent.Test
{
    public class FilesTesting
    {
        [Fact]
        public void ReadAllBytes()
        {
            var (totalLength, files) = GetFiles();
            using (var ms = ReadWholeContent(files))
            {
                Assert.Equal(totalLength, ms.Length);
            }
        }

        [Fact]
        public void CalculatedShaAndTorrentShaSame()
        {
            var (totalLength, files) = GetFiles();
            byte[] bytes;
            using (var ms = ReadWholeContent(files))
            {
                bytes = ms.ToArray();
            }
            Torrent torrent;
            using (var file = File.Open("torrent.torrent", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    torrent = new TorrentSerializer().Deserialize(ms.ToArray());
                }
            }
            // assuming the file lengths is less than piece key;
            byte[] computedHash;
            using (var sha = SHA1.Create())
            {
                computedHash = sha.ComputeHash(bytes);
            }
            var b = new BString(computedHash);
            Assert.Equal(torrent.Info.Pieces, b);
        }

        [Fact]
        public void TorrentBuilderCorrectlyCalculateHash()
        {
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                using (var file = File.Open("torrent.torrent", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    file.CopyTo(ms);
                }
                bytes = ms.ToArray();
            }
            var torrent = new TorrentSerializer().Deserialize(bytes);
            var before = torrent.Info.Pieces;
            var builder = TorrentBuilder.FromExisting(torrent);
            builder.CalculatePieces(new FSProvider());
            var builded = builder.Build();
            Assert.Equal(before, builded.Info.Pieces);
        }

        [Fact]
        public void TorrentBuilderBuildsCorrectTorrent()
        {
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                using (var file = File.Open("torrent.torrent", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    file.CopyTo(ms);
                }
                bytes = ms.ToArray();
            }
            var torrent = new TorrentSerializer().Deserialize(bytes);
            var builder = new TorrentBuilder(torrent.Encoding ?? Encoding.UTF8);
            foreach(var (path, len) in torrent.Info.Files)
            {
                builder.AddFile(string.Join(Path.DirectorySeparatorChar, path.Select(_ => ((BString)_).ToString())), len);
            }
            builder.SetPieceLength(torrent.Info.PieceLength);
            builder.CalculatePieces(new FSProvider());
            var builded = builder.Build();
            Assert.Equal(builded.Info.Pieces, torrent.Info.Pieces);
        }

        private class FSProvider : IFileStreamProvider
        {
            public Stream Resolve(string relativePath, out bool autoDispose)
            {
                autoDispose = true;
                return File.Open(Path.Join("torrent", relativePath), FileMode.Open, FileAccess.Read, FileShare.Read);
            }
        }

        private (long, List<(Stream, long, bool)>) GetFiles()
        {
            var lst = new List<(Stream, long, bool)>();
            var totalLength = 0L;
            for (var i = 1; i < 6; i++)
            {
                var fileInfo = new FileInfo(Path.Combine("torrent", $"document{i}.txt"));
                var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                lst.Add((stream, fileInfo.Length, true));
                totalLength += fileInfo.Length;
            }

            return (totalLength, lst);
        }

        private MemoryStream ReadWholeContent(List<(Stream, long, bool)> files)
        {
            var ms = new MemoryStream();
            using (var stream = new StreamSequence(files))
            {
                var buff = new byte[128];
                var readed = 0;
                while ((readed = stream.Read(buff, 0, buff.Length)) == buff.Length)
                {
                    ms.Write(buff, 0, readed);
                }
                ms.Write(buff, 0, readed);
            }
            ms.Flush();
            return ms;
        }
    }
}
