using System.Globalization;

namespace Net.Torrent.BEncode
{
    public class BNumber : IBEncodedObject
    {
        /// <inheritdoc/>
        public BEncodeType Type => BEncodeType.Number;
        internal string _asciiValue { get; private set; }

        internal BNumber(string value)
        {
            _asciiValue = value;
        }

        // <summary>
        /// Creates instance of <see cref="BNumber"/> with supplied value
        /// </summary>
        public BNumber(int value)
        {
            _asciiValue = value.ToString(CultureInfo.InvariantCulture);
        }

        // <summary>
        /// Creates instance of <see cref="BNumber"/> with supplied value
        /// </summary>
        public BNumber(long value)
        {
            _asciiValue = value.ToString(CultureInfo.InvariantCulture);
        }

        // <summary>
        /// Creates instance of <see cref="BNumber"/> with supplied value
        /// </summary>
        public BNumber(double value)
        {
            _asciiValue = value.ToString(CultureInfo.InvariantCulture);
        }

        public static explicit operator BNumber(int value)
        {
            return new BNumber(value);
        }

        public static explicit operator BNumber(long value)
        {
            return new BNumber(value);
        }

        public static explicit operator BNumber(double value)
        {
            return new BNumber(value);
        }

        public static explicit operator BNumber(uint value)
        {
            return new BNumber(value);
        }

        public static explicit operator BNumber(ulong value)
        {
            return new BNumber(value);
        }

        public static implicit operator int(BNumber number)
        {
            return int.Parse(number._asciiValue, CultureInfo.InvariantCulture);
        }

        public static implicit operator long(BNumber number)
        {
            return long.Parse(number._asciiValue, CultureInfo.InvariantCulture);
        }

        public static implicit operator uint(BNumber number)
        {
            return uint.Parse(number._asciiValue, CultureInfo.InvariantCulture);
        }

        public static implicit operator ulong(BNumber number)
        {
            return ulong.Parse(number._asciiValue, CultureInfo.InvariantCulture);
        }

        public static implicit operator double(BNumber number)
        {
            return double.Parse(number._asciiValue, CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return _asciiValue == ((BNumber)obj)._asciiValue;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _asciiValue.GetHashCode();
        }
    }
}
