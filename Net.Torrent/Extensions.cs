using Net.Torrent.BEncode;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Net.Torrent
{
    public static class Extensions
    {
        /// <summary>
        /// Gets info section hash
        /// </summary>
        /// <param name="torrent"><see cref="Torrent"/></param>
        /// <returns>Hash of the info section</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="torrent"/> is null</exception>
        public static byte[] GetInfoHash(this Torrent torrent)
        {
            torrent = torrent ?? throw new ArgumentNullException(nameof(torrent));
            var dic = ParserHelper.GetInfoDictionary(torrent.Info);
            using (var ms = new MemoryStream())
            {
                using (var sha1 = SHA1.Create())
                {
                    new BEncodeSerializer().Serialize(ms, dic);
                    return sha1.ComputeHash(ms.ToArray());
                }
            }
        }
    }
}
