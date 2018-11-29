using Net.Torrent.BEncode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Net.Torrent.Test
{
    public class BEncodeTests
    {
        [Theory]
        [InlineData("i42e", 42)]
        [InlineData("i-34e", -34)]
        public void ReadsInt(string input, int value)
        {
            var bytes = new ReadOnlySpan<byte>(Encoding.ASCII.GetBytes(input));
            var reader = new BEncodeSerializer();
            var cursor = 0;
            var element = (BNumber)reader.Deserialize(bytes, ref cursor);
            Assert.Equal(BEncodeType.Number, element.Type);
            Assert.Equal(value, element);
        }

        [Theory]
        [InlineData("i42.4e", 42.4)]
        [InlineData("i-34.2e", -34.2)]
        public void ReadsDouble(string input, double value)
        {
            var bytes = new ReadOnlySpan<byte>(Encoding.ASCII.GetBytes(input));
            var reader = new BEncodeSerializer();
            var cursor = 0;
            var element = (BNumber)reader.Deserialize(bytes, ref cursor);
            Assert.Equal(BEncodeType.Number, element.Type);
            Assert.Equal(value, element);
        }

        [Theory]
        [InlineData("4:spam", "spam")]
        [InlineData("3:foo", "foo")]
        [InlineData("5:hello", "hello")]
        public void ReadsString(string input, string expected)
        {
            var bytes = new ReadOnlySpan<byte>(Encoding.ASCII.GetBytes(input));
            var reader = new BEncodeSerializer();
            var cursor = 0;
            var element = (BString)reader.Deserialize(bytes, ref cursor);
            Assert.Equal(BEncodeType.String, element.Type);
            Assert.Equal(expected, element.ToString());
        }

        [Theory]
        [InlineData("l3:foo3:bare", new[] { "foo", "bar" })]
        [InlineData("l3:buze", new[] { "buz" })]
        public void ReadsListSimple(string input, string[] values)
        {
            var bytes = new ReadOnlySpan<byte>(Encoding.ASCII.GetBytes(input));
            var reader = new BEncodeSerializer();
            var cursor = 0;
            var element = (BList)reader.Deserialize(bytes, ref cursor);
            Assert.Equal(BEncodeType.List, element.Type);
            Assert.Equal(values.Length, element.Count);
            for (var i = 0; i < values.Length; i++)
            {
                Assert.Equal(BEncodeType.String, element[i].Type);
                Assert.Equal(values[i], (BString)element[i]);
            }
        }

        [Fact]
        public void ReadsListOfLists()
        {
            var bytes = Encoding.ASCII.GetBytes("l3:foo3:barl3:buz3:baree");
            var reader = new BEncodeSerializer();
            var cursor = 0;
            var element = (BList)reader.Deserialize(bytes, ref cursor);
            Assert.Equal(BEncodeType.List, element.Type);
            var elem0 = element[0];
            Assert.Equal(BEncodeType.String, elem0.Type);
            Assert.Equal("foo", (BString)elem0);
            var elem1 = element[1];
            Assert.Equal(BEncodeType.String, elem1.Type);
            Assert.Equal("bar", (BString)elem1);
            var elem2 = element[2];
            Assert.Equal(BEncodeType.List, elem2.Type);
            var lst = (BList)elem2;
            Assert.Equal(2, lst.Count);
            var item0 = lst[0];
            Assert.Equal(BEncodeType.String, item0.Type);
            Assert.Equal("buz", (BString)item0);
            var item1 = lst[1];
            Assert.Equal(BEncodeType.String, item1.Type);
            Assert.Equal("bar", (BString)item1);
        }

        [Fact]
        public void ReadListOfDictionaries()
        {
            var bytes = Encoding.ASCII.GetBytes("ld3:foo3:bared3:buz3:butee");
            var reader = new BEncodeSerializer();
            var cursor = 0;
            var element = (BList)reader.Deserialize(bytes, ref cursor);
            Assert.Equal(BEncodeType.List, element.Type);
            Assert.Equal(2, element.Count);
            var elem0 = (BDictionary)element[0];
            Assert.Equal(BEncodeType.Dictionary, elem0.Type);
            Assert.Single(elem0.Keys);
            Assert.Single(elem0.Values);
            Assert.Equal("foo", elem0.Keys.First());
            Assert.Equal("bar", (BString)elem0.Values.First());
            var elem1 = (BDictionary)element[1];
            Assert.Equal(BEncodeType.Dictionary, elem1.Type);
            Assert.Single(elem1.Keys);
            Assert.Single(elem1.Values);
            Assert.Equal("buz", elem1.Keys.First());
            Assert.Equal("but", (BString)elem1.Values.First());
        }

        [Theory]
        [InlineData("d3:foo3:bare", new [] { "foo", "bar" })]
        [InlineData("d3:bar3:foo3:buz3:tooe", new [] {"bar", "foo", "buz", "too"})]
        public void ReadDictionarySimple(string input, string[] values)
        {
            var bytes = new ReadOnlySpan<byte>(Encoding.ASCII.GetBytes(input));
            var reader = new BEncodeSerializer();
            var cursor = 0;
            var element = (BDictionary)reader.Deserialize(bytes, ref cursor);
            Assert.Equal(BEncodeType.Dictionary, element.Type);
            var dic = new Dictionary<string, string>();
            for(var i=0;i< values.Length;i+=2)
            {
                var key = values[i];
                var value = values[i + 1];
                dic.Add(key, value);
            }
            Assert.Equal(dic.Count, element.Count);
            foreach(var (key, value) in element)
            {
                Assert.True(dic.ContainsKey(key));
                var val = (BString)element[key];
                Assert.Equal(BEncodeType.String, val.Type);
                Assert.Equal(dic[key], val);
            }
        }
    }
}
