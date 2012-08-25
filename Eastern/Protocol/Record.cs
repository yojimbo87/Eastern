

namespace Eastern.Protocol
{
    internal class Record
    {
        internal ORecordType Type { get; set; }
        internal int Version { get; set; }
        internal byte[] Content { get; set; }
    }
}
