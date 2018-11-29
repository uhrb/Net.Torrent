using System.Collections.Generic;

namespace Net.Torrent.BEncode
{
    /// <summary>
    /// BEncoded dictionary implementation
    /// </summary>
    public class BDictionary : SortedDictionary<BString, IBEncodedObject>, IBEncodedObject
    {
        /// <inheritdoc/>
        public BEncodeType Type => BEncodeType.Dictionary;

        /// <summary>
        /// Creates instance of <see cref="BDictionary"/>
        /// </summary>
        public BDictionary(): base(BStringComparer.Instance)
        {
        }
           
        /// <summary>
        /// Creates instance of <see cref="BDictionary"/>
        /// </summary>
        /// <param name="dictionary">Dictionary to copy from</param>
        public BDictionary(IDictionary<BString, IBEncodedObject> dictionary) : base(dictionary, BStringComparer.Instance)
        { 
        }
    }
}
