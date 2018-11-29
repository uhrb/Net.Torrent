using Net.Torrent.BEncode;
using System.Text;

namespace Net.Torrent
{
    public static class ExtensionKeys
    {
        public static readonly BString Comment = new BString(Encoding.ASCII.GetBytes("comment"));
        public static readonly BString AnnounceList = new BString(Encoding.ASCII.GetBytes("announce-list"));
        public static readonly BString CreatedBy = new BString(Encoding.ASCII.GetBytes("created by"));
        public static readonly BString CreationDate = new BString(Encoding.ASCII.GetBytes("creation date"));
        public static readonly BString Publisher = new BString(Encoding.ASCII.GetBytes("publisher"));
        public static readonly BString PublisherUrl = new BString(Encoding.ASCII.GetBytes("publisher-url"));
    }
}
