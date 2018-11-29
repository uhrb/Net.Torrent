using System.IO;

namespace Net.Torrent
{
    /// <summary>
    /// File stream resolver interface
    /// </summary>
    public interface IFileStreamProvider
    {
        /// <summary>
        /// Resolve relative path to the stream
        /// </summary>
        /// <param name="relativePath">Relative path</param>
        /// <param name="autoDispose">Should stream be disposed immidately after reading on it complete</param>
        /// <returns><see cref="Stream"/></returns>
        Stream Resolve(string relativePath, out bool autoDispose);
    }
}
