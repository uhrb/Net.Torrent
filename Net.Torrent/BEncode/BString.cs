using System;
using System.Linq;
using System.Text;

namespace Net.Torrent.BEncode
{
    public class BString : IBEncodedObject
    {
        public BEncodeType Type => BEncodeType.String;

        internal byte[] Bytes { get; }
        private int? _hashCode = null;

        public BString(byte[] bytes)
        {
            Bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
        }

        public BString(string str, Encoding encoding)
        {
            encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            if (string.IsNullOrEmpty(str))
            {
                Bytes = new byte[0];
            }
            else
            {
                Bytes = encoding.GetBytes(str);
            }
        }

        public static implicit operator string(BString str)
        {
            return str?.ToString();
        }

        public override string ToString()
        {
            return ToString(Encoding.UTF8);
        }


        public string ToString(Encoding encoding)
        {
            return encoding.GetString(Bytes);
        }

        public override bool Equals(object obj)
        {
            return Bytes.SequenceEqual(((BString)obj).Bytes);
        }

        public override int GetHashCode()
        {
            if (_hashCode != null)
            {
                return _hashCode.Value;
            }

            unchecked
            {
                var prime = 16777619u;
                var hash = 2166136261;
                foreach (var byt in Bytes)
                {
                    hash ^= byt;
                    hash *= prime;
                }
                _hashCode = (int)hash;
            }

            return _hashCode.Value;
        }

        public static bool operator ==(BString str1, BString str2)
        {
            if (ReferenceEquals(str1, str2))
            {
                return true;
            }
            var str1Null = ReferenceEquals(str1, null);
            var str2Null = ReferenceEquals(str2, null);
            if (str1Null && !str2Null)
            {
                return false;
            }
            if (!str1Null && str2Null)
            {
                return false;
            }

            if (str1.GetHashCode() == str2.GetHashCode())
            {
                return str1.Equals(str2);
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(BString str1, BString str2)
        {
            return !(str1 == str2);
        }
    }
}
