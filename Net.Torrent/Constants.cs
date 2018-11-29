using Net.Torrent.BEncode;
using System.Text;

namespace Net.Torrent
{
    /// <summary>
    /// Torrent dictionary keys
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Info section key
        /// </summary>
        public static readonly BString InfoNameKey = new BString(Encoding.ASCII.GetBytes("name"));

        /// <summary>
        /// Piece Length key
        /// </summary>
        public static readonly BString InfoPieceLengthKey = new BString(Encoding.ASCII.GetBytes("piece length"));

        /// <summary>
        /// Length key, in Info section
        /// </summary>
        public static readonly BString InfoLengthKey = new BString(Encoding.ASCII.GetBytes("length"));

        /// <summary>
        /// File path key in Files section of Info section
        /// </summary>
        public static readonly BString InfoFilePathKey = new BString(Encoding.ASCII.GetBytes("path"));

        /// <summary>
        /// File length key, in Files section of Info section
        /// </summary>
        public static readonly BString InfoFileLengthKey = InfoLengthKey;

        /// <summary>
        /// Files key in Info section
        /// </summary>
        public static readonly BString InfoFilesKey = new BString(Encoding.ASCII.GetBytes("files"));

        /// <summary>
        /// Pieces key in Info section
        /// </summary>
        public static readonly BString InfoPiecesKey = new BString(Encoding.ASCII.GetBytes("pieces"));

        /// <summary>
        /// Announce key
        /// </summary>
        public static readonly BString AnnounceKey = new BString(Encoding.ASCII.GetBytes("announce"));

        /// <summary>
        /// Info section key
        /// </summary>
        public static readonly BString InfoKey = new BString(Encoding.ASCII.GetBytes("info"));

        /// <summary>
        /// Encoding key
        /// </summary>
        public static readonly BString EncodingKey = new BString(Encoding.ASCII.GetBytes("encoding"));
    }
}
