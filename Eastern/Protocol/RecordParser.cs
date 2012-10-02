using System;
using System.Reflection;
using System.Globalization;

namespace Eastern.Protocol
{
    internal static class RecordParser
    {
        internal static string SerializeObject<T>(T objectToSerialize) 
        {
            Type type = objectToSerialize.GetType();
            string serializedString = type.Name + "@";

            return SerializeObject(serializedString, type.GetProperties());
        }

        private static string SerializeObject(string serializedString, PropertyInfo[] properties)
        {
            if ((properties != null) && (properties.Length > 0))
            {
                for (int i = 0; i < properties.Length; i++) //PropertyInfo property in properties)
                {
                    PropertyInfo property = properties[i];

                    serializedString += property.Name + ":";

                    switch (Type.GetTypeCode(property.PropertyType))
                    {
                        case TypeCode.Boolean:
                            serializedString += property.GetValue(null, null).ToString().ToLower();
                            break;
                        case TypeCode.Byte:
                            serializedString += property.GetValue(null, null).ToString() + "b";
                            break;
                        case TypeCode.Int16:
                            serializedString += property.GetValue(null, null).ToString() + "s";
                            break;
                        case TypeCode.Int32:
                            serializedString += property.GetValue(null, null).ToString();
                            break;
                        case TypeCode.Int64:
                            serializedString += property.GetValue(null, null).ToString() + "l";
                            break;
                        case TypeCode.Single:
                            serializedString += ((float)property.GetValue(null, null)).ToString(CultureInfo.InvariantCulture) + "f";
                            break;
                        case TypeCode.Double:
                            serializedString += ((double)property.GetValue(null, null)).ToString(CultureInfo.InvariantCulture) + "d";
                            break;
                        case TypeCode.Decimal:
                            serializedString += ((decimal)property.GetValue(null, null)).ToString(CultureInfo.InvariantCulture) + "c";
                            break;
                        case TypeCode.DateTime:
                            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                            serializedString += ((long)((DateTime)property.GetValue(null, null) - unixEpoch).TotalMilliseconds).ToString() + "t";
                            break;
                        case TypeCode.String:
                            serializedString += property.GetValue(null, null).ToString();
                            break;
                        default:
                            break;
                    }

                    if (i < (properties.Length - 1))
                    {
                        serializedString += ",";
                    }
                }
            }

            return serializedString;
        }
    }
}
