using Net.Torrent.BEncode;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Net.Torrent
{
    /// <summary>
    /// Torrent serializer/deserializer
    /// </summary>
    public class TorrentSerializer
    {
        private const string MalformedTorrent = "Mailformed torrent";

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
            var dic = GetDictionary(torrent);
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
            return Parse(torrent, 0);
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

        private static BDictionary GetDictionary(Torrent torrent)
        {
            var encoding = torrent.Encoding ?? Encoding.UTF8;
            var dic = new BDictionary
            {
                { Constants.AnnounceKey, new BString(torrent.Announce.ToString(), encoding) },
                { Constants.EncodingKey, new BString(torrent.Encoding.WebName, encoding) }
            };

            foreach (var (key, value) in torrent.Extensions)
            {
                dic.Add(key, value);
            }

            var infoDic = new BDictionary
            {
                { Constants.InfoPieceLengthKey, torrent.Info.PieceLength },
                { Constants.InfoPiecesKey, torrent.Info.Pieces }
            };

            if (torrent.Info.Name != null)
            {
                infoDic.Add(Constants.InfoNameKey, torrent.Info.Name);
            }

            foreach (var (key, value) in torrent.Info.Extensions)
            {
                infoDic.Add(key, value);
            }

            if (torrent.Info.Files.Count == 1)
            {
                var (path, len) = torrent.Info.Files[0];
                infoDic.Add(Constants.InfoLengthKey, len);
            }
            else
            {
                var lst = new BList(torrent.Info.Files.Count);
                foreach (var (key, value) in torrent.Info.Files)
                {
                    var fdic = new BDictionary
                    {
                        { Constants.InfoFilePathKey, key },
                        { Constants.InfoFileLengthKey, value }
                    };
                    lst.Add(fdic);
                }
                infoDic.Add(Constants.InfoFilesKey, lst);
            }

            dic.Add(Constants.InfoKey, infoDic);

            return dic;
        }

        private Torrent Parse(ReadOnlySpan<byte> bytes, int offset)
        {
            var reader = new BEncodeSerializer();
            var dictionary = (BDictionary)reader.Deserialize(bytes, ref offset);
            var torrent = new Torrent();
            var stringEncoding = Encoding.UTF8;
            if (dictionary.TryGetValue(Constants.EncodingKey, out IBEncodedObject enc))
            {
                stringEncoding = Encoding.GetEncoding((BString)enc);
                torrent.Encoding = stringEncoding;
            }
            else
            {
                stringEncoding = DefaultStringEncoding;
            }

            foreach (var (key, value) in dictionary)
            {
                if (key == Constants.AnnounceKey)
                {
                    var announce = (BString)value;
                    if (string.IsNullOrEmpty(announce))
                    {
                        throw new ParsingException(MalformedTorrent);
                    }
                    torrent.Announce = new Uri((BString)value);
                }
                else if (key == Constants.InfoKey)
                {
                    var dic = (BDictionary)value;
                    torrent.Info = ParseInfoSection(dic);
                }
                else if (key == Constants.EncodingKey)
                {
                    // do nothing;
                }
                else
                {
                    torrent.Extensions.Add(key, value);
                }
            }

            if (torrent.Info == null)
            {
                throw new ParsingException(MalformedTorrent);
            }

            return torrent;
        }

        private TorrentInfo ParseInfoSection(BDictionary dic)
        {
            var info = new TorrentInfo();
            bool? singleFile = null;
            foreach (var (key, value) in dic)
            {
                if (key == Constants.InfoNameKey)
                {
                    info.Name = (BString)value;
                }
                else if (key == Constants.InfoLengthKey)
                {
                    if (singleFile == false)
                    {
                        throw new ParsingException(MalformedTorrent);
                    }
                    singleFile = true;
                    info.Files.Add((null, (BNumber)value));
                }
                else if (key == Constants.InfoPieceLengthKey)
                {
                    info.PieceLength = (BNumber)value;
                }
                else if (key == Constants.InfoPiecesKey)
                {
                    info.Pieces = (BString)value;
                }
                else if (key == Constants.InfoFilesKey)
                {
                    if (singleFile == true)
                    {
                        throw new ParsingException(MalformedTorrent);
                    }
                    singleFile = false;
                    info.Files.AddRange(((BList)value).Select(_ =>
                    {
                        var dict = (BDictionary)_;
                        return ((BList)dict[Constants.InfoFilePathKey], (BNumber)dict[Constants.InfoFileLengthKey]);
                    }));
                }
                else
                {
                    info.Extensions.Add(key, value);
                }
            }

            if (info.Files.Count <= 0 || info.PieceLength <= 0 || info.Pieces == null)
            {
                throw new ParsingException(MalformedTorrent);
            }

            return info;
        }
    }
}
