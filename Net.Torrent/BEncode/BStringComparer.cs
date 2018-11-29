using System.Collections.Generic;
using System.Text;

namespace Net.Torrent.BEncode
{
    /// <summary>
    /// BString comparer to be used with dictionaries
    /// </summary>
    public sealed class BStringComparer : IComparer<BString>
    {
        private BStringComparer()
        {
        }

        /// <summary>
        /// Instance of comparer
        /// </summary>
        public static BStringComparer Instance => new BStringComparer();

        /// <inheritdoc/>
        public int Compare(BString str1, BString str2)
        {
            if (ReferenceEquals(str1, str2))
            {
                return 0;
            }
            var str1Null = ReferenceEquals(str1, null);
            var str2Null = ReferenceEquals(str2, null);
            if (str1Null && !str2Null)
            {
                return -1;
            }
            if (!str1Null && str2Null)
            {
                return 1;
            }

            return string.Compare(str1.ToString(Encoding.ASCII), str2.ToString(Encoding.ASCII));
        }
    }
}
