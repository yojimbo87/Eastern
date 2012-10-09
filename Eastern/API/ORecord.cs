using System;
using System.Reflection;
using System.Globalization;
using System.Linq;
using System.Collections;
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
                    if ((property.PropertyType.IsArray || property.PropertyType.IsGenericType))
                    {
                        IList value = (IList)item.Value;

                        if (value.Count > 0)
                        {
                            object values = Activator.CreateInstance(property.PropertyType, value.Count);

                            for (int i = 0; i < value.Count; i++)
                            {
                                if (property.PropertyType.IsArray)
                                {
                                    ((object[])values)[i] = value[i];
                                }
                                else if (property.PropertyType.IsGenericType && (item.Value is IEnumerable))
                                {
                                    Type t = value[i].GetType();

                                    if (t.IsPrimitive || (t == typeof(string)) || (t == typeof(DateTime)) || (t == typeof(decimal)))
                                    {
                                        ((IList)values).Add(value[i]);
                                    }
                                    else
                                    {
                                        // t is dictionary here which should be NestedClass - Type of value[i] is therefore probably incorrect
                                        object o = Activator.CreateInstance(t, null);
                                        o = value[i];
                                        ((IList)values).Add(o);
                                    }
                                }
                                else
                                {
                                    Type t = value[i].GetType();
                                    object v = Activator.CreateInstance(t, value[i]);

                                    ((IList)values).Add(v);
                                }
                            }

                            property.SetValue(genericObject, values, null);
                        }
                    }
                    else if (property.PropertyType.IsClass && (property.PropertyType.Name != "String"))
                    {
                        object value = Activator.CreateInstance(property.PropertyType);
                        property.SetValue(genericObject, value, null);
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
