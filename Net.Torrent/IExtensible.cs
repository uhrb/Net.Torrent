using Net.Torrent.BEncode;
using System.Collections.Generic;

namespace Net.Torrent
{
    public interface IExtensible
    {
        Dictionary<BString, IBEncodedObject> Extensions { get; }
    }
}
