using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using Eastern.Protocol;

namespace Eastern
{
    public class ORecord
    {
        public ORID ORID { get; set; }
        public ORecordType Type { get; set; }
        public int Version { get; set; }
        public byte[] Content { get; set; }
        public string Class { get; set; }
        public Dictionary<string, object> Fields { get; set; }

        public ORecord()
        {
            Fields = new Dictionary<string, object>();
        }

        public ORecord(ORecordType type, int version, byte[] content)
        {
            Type = type;
            Version = version;
            Content = content;
            Fields = new Dictionary<string, object>();

            Deserialize();
        }

        internal ORecord(Record record)
        {
            ORID = record.ORID;
            Type = record.Type;
            Version = record.Version;
            Content = record.Content;
            Fields = new Dictionary<string, object>();
        }

        // for testing parser logic
        private void Deserialize()
        {
            Record record = new Record();
            record.Type = Type;
            record.Content = Content;

            if (Type == ORecordType.Document)
            {
                ORecord deserializedRecord = RecordParser.DeserializeRecord(record);

                Class = deserializedRecord.Class;
                Fields = deserializedRecord.Fields;
            }
        }
    }
}
