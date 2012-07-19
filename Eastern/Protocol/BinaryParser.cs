﻿using System;
using System.Text;

namespace Eastern.Protocol
{
    internal static class BinaryParser
    {
        internal static byte ToByte(byte[] data)
        {
            return data[0];
        }

        internal static short ToShort(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }

            return BitConverter.ToInt16(data, 0);
        }

        internal static int ToInt(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }

            return BitConverter.ToInt32(data, 0);
        }

        internal static string ToString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        internal static byte[] ToArray(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        internal static byte[] ToArray(byte data)
        {
            return new byte[1] { data };
        }

        internal static byte[] ToArray(int data)
        {
            byte[] binaryData = BitConverter.GetBytes(data);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(binaryData);
            }

            return binaryData;
        }

        internal static byte[] ToArray(short data)
        {
            byte[] binaryData = BitConverter.GetBytes(data);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(binaryData);
            }

            return binaryData;
        }
    }
}
