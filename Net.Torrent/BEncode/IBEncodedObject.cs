namespace Net.Torrent.BEncode
{
    /// <summary>
    /// BEncoded object common interface
    /// </summary>
    public interface IBEncodedObject
    {
        /// <summary>
        /// Gets object type
        /// </summary>
        BEncodeType Type { get; }
    }
}
