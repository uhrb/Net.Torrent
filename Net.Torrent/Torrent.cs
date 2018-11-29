using Net.Torrent.BEncode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Torrent
{
    /// <summary>
    /// Torrent file class
    /// </summary>
    public class Torrent : IExtensible
    {
        /// <inheritdoc/>
        public SortedDictionary<BString, IBEncodedObject> Extensions { get; }

        /// <summary>
        /// Announce URL
        /// </summary>
        public Uri Announce { get; set; }

        /// <summary>
        /// Torrent info section
        /// </summary>
        public TorrentInfo Info { get; set; }

        /// <summary>
        /// String encoding (encoding key)
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Creates instance of <see cref="Torrent"/>
        /// </summary>
        public Torrent()
        {
            Extensions = new SortedDictionary<BString, IBEncodedObject>(BStringComparer.Instance);
        }
    }
}
