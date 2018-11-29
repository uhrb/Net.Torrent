using System.Globalization;
using System.Text;

namespace Net.Torrent.BEncode
{
    public class BNumber : IBEncodedObject
    {
        public BEncodeType Type => BEncodeType.Number;

        internal BNumber()
        {
        }

        public BNumber(int value)
        {
            AsciiValue = Encoding.ASCII.GetString(
                Encoding.Convert(Encoding.Default, Encoding.ASCII, Encoding.Default.GetBytes(value.ToString(CultureInfo.InvariantCulture))));
        }

        public BNumber(long value)
        {
            AsciiValue = Encoding.ASCII.GetString(
                Encoding.Convert(Encoding.Default, Encoding.ASCII, Encoding.Default.GetBytes(value.ToString(CultureInfo.InvariantCulture))));
        }

        public BNumber(double value)
        {
            AsciiValue = Encoding.ASCII.GetString(
                Encoding.Convert(Encoding.Default, Encoding.ASCII, Encoding.Default.GetBytes(value.ToString(CultureInfo.InvariantCulture))));
        }

        internal string AsciiValue { get; set; }

        public static implicit operator int(BNumber number)
        {
            return int.Parse(number.AsciiValue, CultureInfo.InvariantCulture);
        }

        public static implicit operator long(BNumber number)
        {
            return long.Parse(number.AsciiValue, CultureInfo.InvariantCulture);
        }

        public static implicit operator uint(BNumber number)
        {
            return uint.Parse(number.AsciiValue, CultureInfo.InvariantCulture);
        }

        public static implicit operator ulong(BNumber number)
        {
            return ulong.Parse(number.AsciiValue, CultureInfo.InvariantCulture);
        }

        public static implicit operator double(BNumber number)
        {
            return double.Parse(number.AsciiValue, CultureInfo.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            return AsciiValue == ((BNumber)obj).AsciiValue;
        }

        public override int GetHashCode()
        {
            return AsciiValue.GetHashCode();
        }
    }
}
