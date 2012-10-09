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
                    // property is array or generic collection 
                    if ((property.PropertyType.IsArray || property.PropertyType.IsGenericType))
                    {
                        IList collection = (IList)item.Value;

                        if (collection.Count > 0)
                        {
                            // create instance of property type
                            object values = Activator.CreateInstance(property.PropertyType, collection.Count);

                            for (int i = 0; i < collection.Count; i++)
                            {
                                // collection is simple array
                                if (property.PropertyType.IsArray)
                                {
                                    ((object[])values)[i] = collection[i];
                                }
                                // collection is generic
                                else if (property.PropertyType.IsGenericType && (item.Value is IEnumerable))
                                {
                                    Type elementType = collection[i].GetType();

                                    // generic collection consists of basic types
                                    if (elementType.IsPrimitive ||
                                        (elementType == typeof(string)) ||
                                        (elementType == typeof(DateTime)) || 
                                        (elementType == typeof(decimal)))
                                    {
                                        ((IList)values).Add(collection[i]);
                                    }
                                    // generic collection consists of generic type which should be parsed
                                    else
                                    {
                                        
                                        var foo2 = property.PropertyType.GetGenericArguments();

                                        object o = Activator.CreateInstance(property.PropertyType.GetGenericArguments().First(), null);
                                        o = collection[i];

                                        // there needs to be a recursion to set properties to object instance from dictionary
                                        ((IList)values).Add(o);
                                    }
                                }
                                else
                                {
                                    Type t = collection[i].GetType();
                                    object v = Activator.CreateInstance(t, collection[i]);

                                    ((IList)values).Add(v);
                                }
                            }

                            property.SetValue(genericObject, values, null);
                        }
                    }
                    // property is class except string type
                    else if (property.PropertyType.IsClass && (property.PropertyType.Name != "String"))
                    {
                        object value = Activator.CreateInstance(property.PropertyType);
                        // recursion oved dictionary values also here
                        property.SetValue(genericObject, value, null);
                    }
                    // property is basic type
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
