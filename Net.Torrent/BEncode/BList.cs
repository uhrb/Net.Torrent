using System.Collections.Generic;

namespace Net.Torrent.BEncode
{
    public class BList : List<IBEncodedObject>, IBEncodedObject
    {
        /// <inheritdoc/>
        public BEncodeType Type => BEncodeType.List;

        /// <summary>
        /// Creates new instance of <see cref="BList"/>
        /// </summary>
        public BList(): base()
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="BList"/>
        /// </summary>
        /// <param name="capacity">Predefined capacity</param>
        public BList(int capacity): base(capacity)
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="BList"/>
        /// </summary>
        /// <param name="objects">List to fill from</param>
        public BList(IList<IBEncodedObject> objects): base(objects == null ? 0 : objects.Count)
        {
            AddRange(objects);
        }

    }
}
