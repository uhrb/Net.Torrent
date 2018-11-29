using Net.Torrent.BEncode;
using System;

namespace Net.Torrent
{
    public static class TorrentExtensions
    {
        public static bool TryGetExtension<T>(this IExtensible extensible, BString extensionKey, out T value)
            where T : class, IBEncodedObject
        {
            extensible = extensible ?? throw new ArgumentNullException(nameof(extensible));
            value = default;
            if (extensible.Extensions.TryGetValue(extensionKey, out IBEncodedObject obj) == false)
            {
                return false;
            }
            value = obj as T;
            if (value == null)
            {
                return false;
            }
            return true;
        }
    }
}
