using Net.Torrent.BEncode;
using System.Text;

namespace Net.Torrent
{
    /// <summary>
    /// Well-known extensions key
    /// </summary>
    public static class ExtensionKeys
    {
        /// <summary>
        /// Comment extension key
        /// </summary>
        public static readonly BString Comment = new BString(Encoding.ASCII.GetBytes("comment"));

        /// <summary>
        /// Announce-list extension key
        /// </summary>
        public static readonly BString AnnounceList = new BString(Encoding.ASCII.GetBytes("announce-list"));

        /// <summary>
        /// Created-by extension key
        /// </summary>
        public static readonly BString CreatedBy = new BString(Encoding.ASCII.GetBytes("created by"));

        /// <summary>
        /// Creation date extension key
        /// </summary>
        public static readonly BString CreationDate = new BString(Encoding.ASCII.GetBytes("creation date"));

        /// <summary>
        /// Publisher extension key
        /// </summary>
        public static readonly BString Publisher = new BString(Encoding.ASCII.GetBytes("publisher"));

        /// <summary>
        /// Publisher url extension key
        /// </summary>
        public static readonly BString PublisherUrl = new BString(Encoding.ASCII.GetBytes("publisher-url"));

        /// <summary>
        /// Private flag key
        /// </summary>
        public static readonly BString Private = new BString(Encoding.ASCII.GetBytes("private"));
    }
}
