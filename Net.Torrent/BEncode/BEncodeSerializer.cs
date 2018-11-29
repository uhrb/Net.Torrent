using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Net.Torrent.BEncode
{
    /// <summary>
    /// Serializer/Deserializer to/from b-encode format
    /// </summary>
    public class BEncodeSerializer
    {
        /// <summary>
        /// Serialize object into stream
        /// </summary>
        /// <param name="stream">Stream, to serialize to</param>
        /// <param name="obj"></param>
        public void Serialize(Stream stream, IBEncodedObject obj)
        {
            WriteElement(stream, obj);
        }

        public IBEncodedObject Deserialize(ReadOnlySpan<byte> from, ref int offset)
        {
            return ReadElement(from, ref offset);
        }

        private void WriteElement(Stream stream, IBEncodedObject elem)
        {
            switch (elem.Type)
            {
                case BEncodeType.Dictionary:
                    WriteDictionary(stream, (BDictionary)elem);
                    break;
                case BEncodeType.List:
                    WriteList(stream, (BList)elem);
                    break;
                case BEncodeType.Number:
                    WriteNumber(stream, (BNumber)elem);
                    break;
                case BEncodeType.String:
                    WriteString(stream, (BString)elem);
                    break;
                default:
                    throw new NotSupportedException($"Element type {elem.Type.ToString()} is not supported");

            }
        }

        private void WriteString(Stream stream, BString str)
        {
            var slen = str._bytes.Length.ToString();
            var len = Encoding.ASCII.GetBytes(slen);
            stream.Write(len);
            stream.WriteByte((byte)':');
            stream.Write(str._bytes);
        }

        private void WriteNumber(Stream stream, BNumber number)
        {
            var bytes = Encoding.ASCII.GetBytes(number._asciiValue);
            stream.WriteByte((byte)'i');
            stream.Write(bytes);
            stream.WriteByte((byte)'e');
        }

        private void WriteList(Stream stream, BList list)
        {
            stream.WriteByte((byte)'l');
            foreach (var element in list)
            {
                WriteElement(stream, element);
            }
            stream.WriteByte((byte)'e');
        }

        private void WriteDictionary(Stream stream, BDictionary dic)
        {
            stream.WriteByte((byte)'d');
            foreach (var (key, value) in dic)
            {
                WriteString(stream, key);
                WriteElement(stream, value);
            }
            stream.WriteByte((byte)'e');
        }

        private IBEncodedObject ReadElement(ReadOnlySpan<byte> bytes, ref int cursor)
        {
            try
            {
                if (cursor >= bytes.Length)
                {
                    return null;
                }

                var key = bytes[cursor];
                switch (key)
                {
                    case (byte)'i':
                        return ReadNumber(bytes, ref cursor);
                    case (byte)'l':
                        return ReadList(bytes, ref cursor);
                    case (byte)'d':
                        return ReadDictionary(bytes, ref cursor);
                    default:
                        return ReadString(bytes, ref cursor);
                }
            }
            catch (Exception e)
            {
                throw new ParsingException($"Error during parsing encoded value at {cursor}", e);
            }
        }

        private BDictionary ReadDictionary(ReadOnlySpan<byte> bytes, ref int cursor)
        {
            cursor++;
            var dict = new SortedDictionary<BString, IBEncodedObject>(BStringComparer.Instance);
            while (bytes[cursor] != (byte)'e')
            {
                var key = ReadString(bytes, ref cursor);
                var value = ReadElement(bytes, ref cursor);
                dict.Add(key, value);
            }
            cursor++;
            return new BDictionary(dict);
        }

        private BList ReadList(ReadOnlySpan<byte> bytes, ref int cursor)
        {
            var start = ++cursor;
            var lst = new List<IBEncodedObject>();
            while (bytes[cursor] != (byte)'e')
            {
                lst.Add(ReadElement(bytes, ref cursor));
            }
            cursor++;
            return new BList(lst);
        }

        private BString ReadString(ReadOnlySpan<byte> bytes, ref int cursor)
        {
            var start = cursor;
            // reading until size end. no bound check
            while (bytes[cursor] != ':')
            {
                cursor++;
            }
            var selected = bytes.Slice(start, cursor - start);
            var stringLength = int.Parse(Encoding.ASCII.GetString(selected));
            cursor++;
            var stringBytes = bytes.Slice(cursor, stringLength);
            cursor += stringLength;
            return new BString(stringBytes.ToArray());
        }

        private BNumber ReadNumber(ReadOnlySpan<byte> bytes, ref int cursor)
        {
            var start = ++cursor;
            // reading until stop. no bound check
            while (bytes[cursor] != 'e')
            {
                cursor++;
            }
            var selected = bytes.Slice(start, cursor - start);
            cursor++;
            return new BNumber(Encoding.ASCII.GetString(selected));
        }
    }
}
