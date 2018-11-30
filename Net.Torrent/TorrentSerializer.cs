using Net.Torrent.BEncode;
using System;
using System.IO;
using System.Text;

namespace Net.Torrent
{
    /// <summary>
    /// Torrent serializer/deserializer
    /// </summary>
    public class TorrentSerializer
    {
        /// <summary>
        /// Default encoding used for strings
        /// </summary>
        public Encoding DefaultStringEncoding { get; }

        /// <summary>
        /// Creates instance of <see cref="TorrentSerializer"/> with default encoding
        /// </summary>
        public TorrentSerializer() : this(Encoding.UTF8)
        {
        }

        /// <summary>
        /// Creates instance of <see cref="TorrentSerializer"/>
        /// </summary>
        /// <param name="defaultStringEncoding">Default string encoding</param>
        /// <exception cref="ArgumentNullException">When <paramref name="defaultStringEncoding"/> is null</exception>
        public TorrentSerializer(Encoding defaultStringEncoding)
        {
            DefaultStringEncoding = defaultStringEncoding ?? throw new ArgumentNullException(nameof(defaultStringEncoding));
        }

        /// <summary>
        /// Serializes torrent to stream
        /// </summary>
        /// <param name="stream"><see cref="Stream"/> to serialize to</param>
        /// <param name="torrent"><see cref="Torrent"/> to serialize</param>
        /// <exception cref="ArgumentNullException">When <paramref name="stream"/> or <paramref name="torrent"/> is null</exception>
        public void Serialize(Stream stream, Torrent torrent)
        {
            torrent = torrent ?? throw new ArgumentNullException(nameof(torrent));
            stream = stream ?? throw new ArgumentNullException(nameof(stream));
            var dic = ParserHelper.GetTorrentDictionary(torrent);
            var writer = new BEncodeSerializer();
            writer.Serialize(stream, dic);
        }

        /// <summary>
        /// Deserializes torrent from memory
        /// </summary>
        /// <param name="torrent">Torrent bytes</param>
        /// <returns><see cref="Torrent"/> instance</returns>
        /// <exception cref="ParsingException">When torrent malformed</exception>
        public Torrent Deserialize(ReadOnlySpan<byte> torrent)
        {
            return ParserHelper.ParseTorrent(torrent, 0, DefaultStringEncoding);
        }

        /// <summary>
        /// Deserialize torrent from stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns><see cref="Torrent"/></returns>
        /// <exception cref="ArgumentNullException">When <paramref name="stream"/> is null</exception>
        public Torrent Deserialize(Stream stream)
        {
            stream = stream ?? throw new ArgumentNullException(nameof(stream));
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return Deserialize(ms.ToArray());
            }
        }


    }
}
