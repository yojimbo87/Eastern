using System;
using System.Reflection;

namespace Eastern.Protocol
{
    internal static class RecordParser
    {
        internal static string MarshalObject<T>(T objectToMarshal) 
        {
            string marshaledObjectString = "";

            Type type = objectToMarshal.GetType();

            foreach (PropertyInfo property in type.GetProperties())
            {
                
            }

            return marshaledObjectString;
        }

        //private static string MarshalEmbeddedObject
    }
}
