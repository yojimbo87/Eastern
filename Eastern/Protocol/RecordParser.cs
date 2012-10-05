using System;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Text;

namespace Eastern.Protocol
{
    internal static class RecordParser
    {
        internal static string SerializeObject(object objectToSerialize) 
        {
            Type type = objectToSerialize.GetType();
            string serializedString = type.Name + "@";

            serializedString += SerializeObject(objectToSerialize, type.GetProperties());

            return serializedString;
            //return Encoding.UTF8.GetBytes(SerializeObject(serializedString, type.GetProperties(), false));
        }

        private static string SerializeObject(object objectToSerialize, PropertyInfo[] properties)
        {
            string serializedString = "";

            if ((properties != null) && (properties.Length > 0))
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo property = properties[i];
                    
                    serializedString += property.Name + ":";
                    serializedString += SerializeValue(property.GetValue(objectToSerialize, null));

                    if (i < (properties.Length - 1))
                    {
                        serializedString += ",";
                    }
                }
            }

            return serializedString;
        }

        private static string SerializeValue(object value)
        {
            string serializedString = "";

            if (value != null)
            {
                Type valueType = value.GetType();

                switch (Type.GetTypeCode(valueType))
                {
                    case TypeCode.Empty:
                        // null case is empty
                        break;
                    case TypeCode.Boolean:
                        serializedString += value.ToString().ToLower();
                        break;
                    case TypeCode.Byte:
                        serializedString += value.ToString() + "b";
                        break;
                    case TypeCode.Int16:
                        serializedString += value.ToString() + "s";
                        break;
                    case TypeCode.Int32:
                        serializedString += value.ToString();
                        break;
                    case TypeCode.Int64:
                        serializedString += value.ToString() + "l";
                        break;
                    case TypeCode.Single:
                        serializedString += ((float)value).ToString(CultureInfo.InvariantCulture) + "f";
                        break;
                    case TypeCode.Double:
                        serializedString += ((double)value).ToString(CultureInfo.InvariantCulture) + "d";
                        break;
                    case TypeCode.Decimal:
                        serializedString += ((decimal)value).ToString(CultureInfo.InvariantCulture) + "c";
                        break;
                    case TypeCode.DateTime:
                        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        serializedString += ((long)((DateTime)value - unixEpoch).TotalMilliseconds).ToString() + "t";
                        break;
                    case TypeCode.String:
                    case TypeCode.Char:
                        // strings must escape these characters:
                        // " -> \"
                        // \ -> \\
                        string stringValue = value.ToString();
                        // escape quotes
                        stringValue = stringValue.Replace("\\", "\\\\");
                        // escape backslashes
                        stringValue = stringValue.Replace("\"", "\\" + "\"");

                        serializedString += "\"" + stringValue + "\"";
                        break;
                    case TypeCode.Object:
                        if ((valueType.IsArray) || (valueType.IsGenericType))
                        {
                            serializedString += "[";

                            IEnumerable collection = (IEnumerable)value;

                            foreach (object val in collection)
                            {
                                if (valueType.IsArray)
                                {
                                    serializedString += SerializeValue(val);
                                }
                                else
                                {
                                    //Type valType = val.GetType();
                                    //serializedString += SerializeObject(val, valType.GetProperties());
                                    serializedString += SerializeValue(val);
                                }

                                serializedString += ",";
                            }

                            // remove last comma from currently parsed collection
                            if (serializedString[serializedString.Length - 1] == ',')
                            {
                                serializedString = serializedString.Remove(serializedString.Length - 1);
                            }

                            serializedString += "]";
                        }
                        else if (valueType.IsClass)
                        {
                            serializedString += "(";
                            serializedString += SerializeObject(value, valueType.GetProperties());
                            serializedString += ")";
                        }
                        break;
                    default:
                        break;
                }
            }

            return serializedString;
        }
    }
}
