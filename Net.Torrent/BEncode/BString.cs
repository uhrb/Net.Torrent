using System;
using System.Linq;
using System.Text;

namespace Net.Torrent.BEncode
{
    public class BString : IBEncodedObject
    {
        /// <inheritdoc/>
        public BEncodeType Type => BEncodeType.String;

        internal byte[] _bytes { get; private set; }
        private int? _hashCode = null;

        /// <summary>
        /// Creates instance of <see cref="BString"/> with supplied byte array value
        /// </summary>
        /// <param name="bytes">Byte array</param>
        /// <exception cref="ArgumentNullException">When <paramref name="bytes"/> is null</exception>
        public BString(byte[] bytes)
        {
            _bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
        }

        /// <summary>
        /// Creates instance of <see cref="BString"/> with supplied byte array value
        /// </summary>
        /// <param name="str">String to create from</param>
        /// <param name="encoding">Encoding of the string</param>
        /// <exception cref="ArgumentNullException">When <paramref name="encoding"/> is null</exception>
        public BString(string str, Encoding encoding)
        {
            encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            if (string.IsNullOrEmpty(str))
            {
                _bytes = new byte[0];
            }
            else
            {
                _bytes = encoding.GetBytes(str);
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

        /// <summary>
        /// Returns string using particular encoding. No encoding conversion happen
        /// </summary>
        /// <param name="encoding">Encoding to be used</param>
        /// <returns>String value</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="encoding"/> is null</exception>
        public string ToString(Encoding encoding)
        {
            encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            return encoding.GetString(_bytes);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return _bytes.SequenceEqual(((BString)obj)._bytes);
        }

        /// <inheritdoc/>
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
                foreach (var byt in _bytes)
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
