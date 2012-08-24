using Eastern.Protocol;

namespace Eastern
{
    public class ORecord
    {
        public string Type { get; set; }
        public int Version { get; set; }
        public byte[] Content { get; set; }

        internal ORecord(Record record)
        {
            Type = record.Type.ToString();
            Version = record.Version;
            Content = record.Content;
        }
    }
}
