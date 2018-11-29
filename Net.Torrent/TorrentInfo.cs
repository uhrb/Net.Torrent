using Net.Torrent.BEncode;
using System.Collections.Generic;

namespace Net.Torrent
{
    /// <summary>
    /// Torrent info section
    /// </summary>
    public class TorrentInfo : IExtensible
    {
        /// <summary>
        /// Name key value
        /// </summary>
        public BString Name { get; set; }

        /// <summary>
        /// Piece length
        /// </summary>
        public BNumber PieceLength { get; set; }

        /// <summary>
        /// Extensions
        /// </summary>
        public SortedDictionary<BString, IBEncodedObject> Extensions { get; }

        /// <summary>
        /// Files
        /// </summary>
        public List<(BList, BNumber)> Files { get; }

        /// <summary>
        /// Pieces SHA1 concatenated
        /// </summary>
        public BString Pieces { get; set; }

        /// <summary>
        /// Creates instance of <see cref="TorrentInfo"/>
        /// </summary>
        public TorrentInfo()
        {
            Extensions = new SortedDictionary<BString, IBEncodedObject>(BStringComparer.Instance);
            Files = new List<(BList, BNumber)>();
        }
    }
}
