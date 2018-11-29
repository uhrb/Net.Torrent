using Net.Torrent.BEncode;
using System.Collections.Generic;

namespace Net.Torrent
{
    /// <summary>
    /// General extensible object
    /// </summary>
    public interface IExtensible
    {
        /// <summary>
        /// Extensions dictionary
        /// </summary>
        SortedDictionary<BString, IBEncodedObject> Extensions { get; }
    }
}
