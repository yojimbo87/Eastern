using System;
using System.Reflection;
using System.Globalization;
using System.Text;

namespace Eastern.Protocol
{
    internal static class RecordParser
    {
        internal static string SerializeObject(object objectToSerialize) 
        {
            Type type = objectToSerialize.GetType();
            string serializedString = type.Name + "@";

            return SerializeObject(serializedString, objectToSerialize, type.GetProperties(), false);
            //return Encoding.UTF8.GetBytes(SerializeObject(serializedString, type.GetProperties(), false));
        }

        private static string SerializeObject(string serializedString, object objectToSerialize, PropertyInfo[] properties, bool isEmbedded)
        {
            if ((properties != null) && (properties.Length > 0))
            {
                for (int i = 0; i < properties.Length; i++) //PropertyInfo property in properties)
                {
                    PropertyInfo property = properties[i];
                    object propertyValue = property.GetValue(objectToSerialize, null);

                    if (isEmbedded)
                    {
                        serializedString += "(";
                    }

                    serializedString += property.Name + ":";

                    if (propertyValue != null)
                    {
                        switch (Type.GetTypeCode(property.PropertyType))
                        {
                            case TypeCode.Empty:
                                // null case is empty
                                break;
                            case TypeCode.Boolean:
                                serializedString += propertyValue.ToString().ToLower();
                                break;
                            case TypeCode.Byte:
                                serializedString += propertyValue.ToString() + "b";
                                break;
                            case TypeCode.Int16:
                                serializedString += propertyValue.ToString() + "s";
                                break;
                            case TypeCode.Int32:
                                serializedString += propertyValue.ToString();
                                break;
                            case TypeCode.Int64:
                                serializedString += propertyValue.ToString() + "l";
                                break;
                            case TypeCode.Single:
                                serializedString += ((float)propertyValue).ToString(CultureInfo.InvariantCulture) + "f";
                                break;
                            case TypeCode.Double:
                                serializedString += ((double)propertyValue).ToString(CultureInfo.InvariantCulture) + "d";
                                break;
                            case TypeCode.Decimal:
                                serializedString += ((decimal)propertyValue).ToString(CultureInfo.InvariantCulture) + "c";
                                break;
                            case TypeCode.DateTime:
                                DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                                serializedString += ((long)((DateTime)propertyValue - unixEpoch).TotalMilliseconds).ToString() + "t";
                                break;
                            case TypeCode.String:
                            case TypeCode.Char:
                                serializedString += propertyValue.ToString();
                                break;
                            case TypeCode.Object:
                                break;
                            default:
                                break;
                        }
                    }

                    if (i < (properties.Length - 1))
                    {
                        serializedString += ",";
                    }
                    else
                    {
                        if (isEmbedded)
                        {
                            serializedString += ")";
                        }
                    }
                }
            }

            return serializedString;
        }
    }
}
