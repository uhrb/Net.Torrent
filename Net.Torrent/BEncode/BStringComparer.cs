using System.Collections.Generic;
using System.Text;

namespace Net.Torrent.BEncode
{
    internal sealed class BStringComparer : IComparer<BString>
    {
        private BStringComparer()
        {
        }

        public static BStringComparer Instance => new BStringComparer();

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
            return string.Compare(Encoding.ASCII.GetString(str1.Bytes), Encoding.ASCII.GetString(str2.Bytes));
        }
    }
}
