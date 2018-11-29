using System.IO;

namespace Net.Torrent
{
    public interface IFileStreamProvider
    {
        Stream Resolve(string relativePath, out bool autoDispose);
    }
}
