using System;
using System.Reflection;
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

        public static byte[] Serialize<T>(T o)
        {
            return RecordParser.SerializeObject(o, o.GetType());
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

        public T ToObject<T>() where T : class, new()
        {
            T genericObject = new T();
            Type genericObjectType = genericObject.GetType();

            foreach (KeyValuePair<string, object> item in Fields)
            {
                PropertyInfo property = genericObjectType.GetProperty(item.Key);

                if (property != null)
                {
                    if (property.PropertyType.IsArray)
                    {
                    }
                    else if (property.PropertyType.IsGenericType)
                    {
                    }
                    else if (property.PropertyType.IsClass && (property.PropertyType.Name != "String"))
                    {
                    }
                    else
                    {
                        property.SetValue(genericObject, item.Value, null);
                    }
                }
            }

            return genericObject;
        }
    }
}
