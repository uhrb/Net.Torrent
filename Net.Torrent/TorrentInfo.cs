using Net.Torrent.BEncode;
using System.Collections.Generic;

namespace Net.Torrent
{
    public class TorrentInfo : IExtensible
    {
        public BString Name { get; set; }
        public BNumber PieceLength { get; set; }
        public Dictionary<BString, IBEncodedObject> Extensions { get; }
        public List<(BList, BNumber)> Files { get; }
        public BString Pieces { get; set; }

        public TorrentInfo()
        {
            Extensions = new Dictionary<BString, IBEncodedObject>();
            Files = new List<(BList, BNumber)>();
        }
    }
}
