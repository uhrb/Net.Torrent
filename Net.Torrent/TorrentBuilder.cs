using Net.Torrent.BEncode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Net.Torrent
{
    /// <summary>
    /// Allows to build torrents
    /// </summary>
    public class TorrentBuilder
    {
        private readonly Encoding _encoding;
        private readonly Dictionary<BString, IBEncodedObject> _main;
        private readonly Dictionary<BString, IBEncodedObject> _info;
        private readonly List<(string, long)> _files;

        private long _pieceLength;
        private string _name;
        private Uri _announce;
        private byte[] _pieces;

        /// <summary>
        /// Creates instance of <see cref="TorrentBuilder"/>
        /// </summary>
        /// <param name="stringEncoding">Strings encoding</param>
        /// <exception cref="ArgumentNullException">When <paramref name="stringEncoding"/> is null</exception>
        public TorrentBuilder(Encoding stringEncoding)
        {
            _encoding = stringEncoding ?? throw new ArgumentNullException(nameof(stringEncoding));
            _main = new Dictionary<BString, IBEncodedObject>();
            _info = new Dictionary<BString, IBEncodedObject>();
            _files = new List<(string, long)>();
        }

        /// <summary>
        /// Sets Name key
        /// </summary>
        /// <param name="name">Name key value</param>
        public void SetName(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Sets Announce key value
        /// </summary>
        /// <param name="announce">Announce URL</param>
        /// <exception cref="ArgumentNullException">When <paramref name="announce"/> is null</exception>
        public void SetAnnounce(Uri announce)
        {
            _announce = announce ?? throw new ArgumentNullException(nameof(announce));
        }

        /// <summary>
        /// Sets main section extension value. If value already exists - it will be overwritten
        /// </summary>
        /// <param name="key">Extension key</param>
        /// <param name="value">Extension value</param>
        public void SetExtension(BString key, IBEncodedObject value)
        {
            if(!_main.TryAdd(key, value))
            {
                _main[key] = value;
            }
        }

        /// <summary>
        /// Sets Info section extension value. If value already exists - it will be overwritten
        /// </summary>
        /// <param name="key">Extension key</param>
        /// <param name="value">Extension value</param>
        public void SetInfoExtension(BString key, IBEncodedObject value)
        {
            if(!_info.TryAdd(key, value))
            {
                _info[key] = value;
            }
        }

        /// <summary>
        /// Sets piece length
        /// </summary>
        /// <param name="length">Length of piece</param>
        public void SetPieceLength(uint length)
        {
            _pieceLength = length;
        }

        /// <summary>
        /// Clears files. Pieces SHA1 hashes will be purged by this operation
        /// </summary>
        public void ClearFiles()
        {
            _files.Clear();
            _pieces = null;
        }

        /// <summary>
        /// Adds file to torrent
        /// </summary>
        /// <param name="relativePath">Relative path inside torrent</param>
        /// <param name="length">File length</param>
        /// <exception cref="ArgumentException">When item with <paramref name="relativePath"/> already exists</exception>
        public void AddFile(string relativePath, long length)
        {
            if (_files.Any(_ => _.Item1 == relativePath))
            {
                throw new ArgumentException($"Item with key {relativePath} already exists", nameof(relativePath));
            }
            _files.Add((relativePath, length));
        }

        /// <summary>
        /// Calculates pieces sha
        /// </summary>
        /// <param name="provider"><see cref="IFileStreamProvider"/></param>
        /// <exception cref="ArgumentNullException">When <paramref name="provider"/> is null</exception>
        /// <exception cref="InvalidOperationException">When number of files is zero</exception>
        /// <exception cref="InvalidDataException">When <paramref name="provider"/> was not able to resolve the stream</exception>
        public void CalculatePieces(IFileStreamProvider provider)
        {
            provider = provider ?? throw new ArgumentNullException(nameof(provider));
            if(_files.Count == 0)
            {
                throw new InvalidOperationException("Cannot calculate SHA1s when files amount is zero");
            }
            var files = new List<(Stream, long, bool)>(_files.Count);
            foreach (var (path, len) in _files)
            {
                var stream = provider.Resolve(path, out bool autoClose);
                if (stream == null)
                {
                    throw new InvalidDataException($"File stream provider was not able to resolve {path}");
                }
                files.Add((stream, len, autoClose));
            }
            using (var streamSequence = new StreamSequence(files))
            {
                var buff = new byte[_pieceLength];
                using (var ms = new MemoryStream())
                {
                    using (var sha = SHA1.Create())
                    {
                        int readed;
                        do
                        {
                            readed = streamSequence.Read(buff, 0, buff.Length);
                            var pieceHash = sha.ComputeHash(buff, 0, readed);
                            ms.Write(pieceHash);

                        } while (readed == buff.Length);
                    }
                    _pieces = ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Builds torrent from current state
        /// </summary>
        /// <returns><see cref="Torrent"/></returns>
        public Torrent Build()
        {
            var encoding = _encoding ?? Encoding.UTF8;
            var pieces = new BString(_pieces);
            var info = new TorrentInfo
            {
                Name = new BString(_name, encoding),
                PieceLength = new BNumber(_pieceLength),
                Pieces = pieces
            };
            foreach (var (key, value) in _info)
            {
                info.Extensions.Add(key, value);
            }
            foreach (var (key, value) in _files)
            {
                var pathlist = new BList(_files.Count);
                foreach (var part in key.Split('/'))
                {
                    var s = new BString(part, encoding);
                    pathlist.Add(s);
                }
                info.Files.Add((pathlist, new BNumber(value)));
            }

            var torrent = new Torrent
            {
                Encoding = _encoding,
                Announce = _announce,
                Info = info
            };

            foreach (var (key, value) in _main)
            {
                torrent.Extensions.Add(key, value);
            }

            return torrent;
        }

        /// <summary>
        /// Creates instance of <see cref="TorrentBuilder"/> from existing torrent file. 
        /// If Encoding is not specified in torrent, UTF-8 used.
        /// </summary>
        /// <param name="torrent"><see cref="Torrent"/> to create from. </param>
        /// <returns>Torrent builder instance</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="torrent"/> is null</exception>
        public static TorrentBuilder FromExisting(Torrent torrent)
        {
            torrent = torrent ?? throw new ArgumentNullException(nameof(torrent));
            var enc = torrent.Encoding ?? Encoding.UTF8;
            var builder = new TorrentBuilder(enc)
            {
                _name = torrent.Info.Name.ToString(),
                _announce = torrent.Announce,
                _pieceLength = torrent.Info.PieceLength,
                _pieces = new byte[torrent.Info.Pieces._bytes.Length]
            };
            Array.Copy(torrent.Info.Pieces._bytes, builder._pieces, torrent.Info.Pieces._bytes.Length);
            foreach (var (key, value) in torrent.Extensions)
            {
                builder._main.Add(key, value);
            }
            foreach (var (key, value) in torrent.Info.Extensions)
            {
                builder._info.Add(key, value);
            }
            foreach (var (pth, len) in torrent.Info.Files)
            {
                var path = string.Join(Path.PathSeparator, pth.Select(_ => ((BString)_).ToString()));
                builder._files.Add((path, len));
            }

            return builder;
        }
    }
}
