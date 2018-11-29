using Net.Torrent.BEncode;
using System.Text;

namespace Net.Torrent
{
    internal static class Constants
    {
        public static readonly BString InfoNameKey = new BString(Encoding.ASCII.GetBytes("name"));
        public static readonly BString InfoPieceLengthKey = new BString(Encoding.ASCII.GetBytes("piece length"));
        public static readonly BString InfoLengthKey = new BString(Encoding.ASCII.GetBytes("length"));
        public static readonly BString InfoFilePathKey = new BString(Encoding.ASCII.GetBytes("path"));
        public static readonly BString InfoFileLengthKey = InfoLengthKey;
        public static readonly BString InfoFilesKey = new BString(Encoding.ASCII.GetBytes("files"));
        public static readonly BString InfoPiecesKey = new BString(Encoding.ASCII.GetBytes("pieces"));
        public static readonly BString AnnounceKey = new BString(Encoding.ASCII.GetBytes("announce"));
        public static readonly BString InfoKey = new BString(Encoding.ASCII.GetBytes("info"));
        public static readonly BString EncodingKey = new BString(Encoding.ASCII.GetBytes("encoding"));
    }
}
