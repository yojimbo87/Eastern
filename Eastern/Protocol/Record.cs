

namespace Eastern.Protocol
{
    internal class Record
    {
        internal ORID ORID { get; set; }
        internal ORecordType Type { get; set; }
        internal int Version { get; set; }
        internal byte[] Content { get; set; }

        internal Record()
        {
            ORID = new ORID();
        }

        internal ORecord Deserialize()
        {
            return RecordParser.DeserializeRecord(this);
        }
    }
}
