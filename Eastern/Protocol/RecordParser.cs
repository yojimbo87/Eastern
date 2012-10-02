using System;
using System.Reflection;

namespace Eastern.Protocol
{
    internal static class RecordParser
    {
        internal static string SerializeObject<T>(T objectToSerialize) 
        {
            string serializedString = "";

            Type type = objectToSerialize.GetType();

            serializedString = SerializeObject(serializedString, type.GetProperties());

            return serializedString;
        }

        private static string SerializeObject(string serializedString, PropertyInfo[] properties)
        {
            if ((properties != null) && (properties.Length > 0))
            {
                foreach (PropertyInfo property in properties)
                {
                    switch (Type.GetTypeCode(property.PropertyType))
                    {
                        case TypeCode.Boolean:
                            break;
                        case TypeCode.Byte:
                            break;
                        case TypeCode.Int16:
                            break;
                        case TypeCode.Int32:
                            break;
                        case TypeCode.Int64:
                            break;
                        case TypeCode.Double:
                            break;
                        case TypeCode.DateTime:
                            break;
                        case TypeCode.String:
                            break;
                        default:
                            break;
                    }
                }
            }

            return serializedString;
        }
    }
}
