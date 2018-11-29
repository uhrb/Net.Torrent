using Net.Torrent.BEncode;
using System;

namespace Net.Torrent
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class TorrentExtensions
    {
        /// <summary>
        /// Tries to get extension value from object
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="extensible">Object to get from</param>
        /// <param name="extensionKey">Extension key. <see cref="ExtensionKeys"/> for well-known extension keys</param>
        /// <param name="value">Value</param>
        /// <returns>Success or not</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="extensible"/> is null</exception>
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
