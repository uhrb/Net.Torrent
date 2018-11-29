using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Net.Torrent.BEncode
{
    public class BDictionary : IBEncodedObject, IReadOnlyDictionary<BString, IBEncodedObject>
    {
        public BEncodeType Type => BEncodeType.Dictionary;

        internal SortedDictionary<BString, IBEncodedObject> Dictionary { get; set; }

        public IBEncodedObject this[BString index]
        {
            get
            {
                return Dictionary[index];
            }
        }

        public bool ContainsKey(BString key) => Dictionary.ContainsKey(key);

        public bool TryGetValue(BString key, out IBEncodedObject value) => Dictionary.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<BString, IBEncodedObject>> GetEnumerator() => Dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Dictionary.GetEnumerator();

        public int Count => Dictionary.Count;

        public IEnumerable<BString> Keys => Dictionary.Keys;

        public IEnumerable<IBEncodedObject> Values => Dictionary.Values;
    }
}
