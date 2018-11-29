using Net.Torrent.BEncode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Net.Torrent
{
    public class TorrentSerializer
    {
        private const string MalformedTorrent = "Mailformed torrent";

        public Encoding DefaultStringEncoding { get; }

        public TorrentSerializer() : this(Encoding.UTF8)
        {
        }

        public TorrentSerializer(Encoding defaultStringEncoding)
        {
            DefaultStringEncoding = defaultStringEncoding ?? throw new ArgumentNullException(nameof(defaultStringEncoding));
        }

        public void Serialize(Stream stream, Torrent torrent)
        {
            torrent = torrent ?? throw new ArgumentNullException(nameof(torrent));
            stream = stream ?? throw new ArgumentNullException(nameof(stream));
            var dic = GetDictionary(torrent);
            var writer = new BEncodeSerializer();
            writer.Serialize(stream, dic);
        }

        private static BDictionary GetDictionary(Torrent torrent)
        {
            var encoding = torrent.Encoding ?? Encoding.UTF8;
            var dic = new BDictionary
            {
                Dictionary = new SortedDictionary<BString, IBEncodedObject>(BStringComparer.Instance)
            };
            dic.Dictionary.Add(Constants.AnnounceKey, new BString(torrent.Announce.ToString(), encoding));
            dic.Dictionary.Add(Constants.EncodingKey, new BString(torrent.Encoding.WebName, encoding));
            foreach (var (key, value) in torrent.Extensions)
            {
                dic.Dictionary.Add(key, value);
            }
            var infoDic = new BDictionary
            {
                Dictionary = new SortedDictionary<BString, IBEncodedObject>(BStringComparer.Instance)
            };
            foreach (var (key, value) in torrent.Info.Extensions)
            {
                infoDic.Dictionary.Add(key, value);
            }
            if (torrent.Info.Name != null)
            {
                infoDic.Dictionary.Add(Constants.InfoNameKey, torrent.Info.Name);
            }
            infoDic.Dictionary.Add(Constants.InfoPieceLengthKey, torrent.Info.PieceLength);
            infoDic.Dictionary.Add(Constants.InfoPiecesKey, torrent.Info.Pieces);
            if (torrent.Info.Files.Count == 1)
            {
                var (path, len) = torrent.Info.Files[0];
                infoDic.Dictionary.Add(Constants.InfoLengthKey, len);
            }
            else
            {
                var lst = new BList
                {
                    Objects = new List<IBEncodedObject>(torrent.Info.Files.Count)
                };
                foreach (var (key, value) in torrent.Info.Files)
                {
                    var fdic = new BDictionary
                    {
                        Dictionary = new SortedDictionary<BString, IBEncodedObject>(BStringComparer.Instance)
                        {
                            { Constants.InfoFilePathKey, key },
                            { Constants.InfoFileLengthKey, value }
                        }
                    };
                    lst.Objects.Add(fdic);
                }
                infoDic.Dictionary.Add(Constants.InfoFilesKey, lst);
            }

            dic.Dictionary.Add(Constants.InfoKey, infoDic);

            return dic;
        }

        public Torrent Deserialize(ReadOnlySpan<byte> torrent)
        {
            return Parse(torrent, 0);
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
