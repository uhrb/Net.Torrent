using Net.Torrent.BEncode;
using System;
using System.Linq;
using System.Text;

namespace Net.Torrent
{
    internal static class ParserHelper
    {
        private const string MalformedTorrent = "Malformed torrent";

        public static BDictionary GetInfoDictionary(TorrentInfo info)
        {
            var infoDic = new BDictionary
            {
                { Constants.InfoPieceLengthKey, info.PieceLength },
                { Constants.InfoPiecesKey, info.Pieces }
            };

            if (info.Name != null)
            {
                infoDic.Add(Constants.InfoNameKey, info.Name);
            }

            foreach (var (key, value) in info.Extensions)
            {
                infoDic.Add(key, value);
            }

            if (info.Files.Count == 1)
            {
                var (path, len) = info.Files[0];
                infoDic.Add(Constants.InfoLengthKey, len);
            }
            else
            {
                var lst = new BList(info.Files.Count);
                foreach (var (key, value) in info.Files)
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

            return infoDic;
        }

        public static BDictionary GetTorrentDictionary(Torrent torrent)
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

            var infoDic = GetInfoDictionary(torrent.Info);

            dic.Add(Constants.InfoKey, infoDic);

            return dic;
        }

        public static Torrent ParseTorrent(ReadOnlySpan<byte> bytes, int offset, Encoding defaultStringEncoding)
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
                stringEncoding = defaultStringEncoding;
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

        public static TorrentInfo ParseInfoSection(BDictionary dic)
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
