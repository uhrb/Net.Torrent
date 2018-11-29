using System.Collections;
using System.Collections.Generic;

namespace Net.Torrent.BEncode
{
    public class BList : IBEncodedObject, IEnumerable<IBEncodedObject>
    {
        public BEncodeType Type => BEncodeType.List;

        internal List<IBEncodedObject> Objects { get; set; }

        public IBEncodedObject this[int index]
        {
            get
            {
                return Objects[index];
            }
        }

        public int Count => Objects.Count;

        public IEnumerator<IBEncodedObject> GetEnumerator() => Objects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Objects.GetEnumerator();
    }
}
