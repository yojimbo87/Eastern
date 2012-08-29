using Eastern.Protocol;

namespace Eastern
{
    public class ORecord
    {
        public ORecordType Type { get; set; }
        public int Version { get; set; }
        public byte[] Content { get; set; }

        public ORecord(ORecordType type, int version, byte[] content)
        {
            Type = type;
            Version = version;
            Content = content;
        }

        internal ORecord(Record record)
        {
            Type = record.Type;
            Version = record.Version;
            Content = record.Content;
        }

        public ODocument ToDocument()
        {
            if (Type == ORecordType.Document)
            {
                ODocument document = RecordParser.ToDocument(BinaryParser.ToString(Content));
                document.Version = Version;

                return document;
            }
            else
            {
                return null;
            }
        }
    }
}
