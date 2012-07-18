using System;
using System.Text;

namespace Eastern.Protocol
{
    internal class BinaryParser
    {
        internal byte ToByte(byte[] data)
        {
            return data[0];
        }

        internal short ToShort(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }

            return BitConverter.ToInt16(data, 0);
        }

        internal int ToInt(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }

            return BitConverter.ToInt32(data, 0);
        }

        internal string ToString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        internal byte[] ToArray(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        internal byte[] ToArray(byte data)
        {
            return new byte[1] { data };
        }

        internal byte[] ToArray(int data)
        {
            byte[] binaryData = BitConverter.GetBytes(data);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(binaryData);
            }

            return binaryData;
        }

        internal byte[] ToArray(short data)
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
