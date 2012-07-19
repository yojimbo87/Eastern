using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

using Eastern.Protocol;
using Eastern.Protocol.Operations;

namespace Eastern.Connection
{
    internal class WorkerConnection
    {
        private TcpClient Socket { get; set; }
        private NetworkStream Stream { get; set; }
        private byte[] ReadBuffer { get; set; }

        internal string Hostname { get; set; }
        internal int HostPort { get; set; }
        internal short ProtocolVersion { get; set; }
        internal int SessionID { get; set; }

        internal WorkerConnection()
        {
            ReadBuffer = new byte[1024];
            ProtocolVersion = 0;
            SessionID = -1;
        }

        internal short Initialize(string hostname, int port)
        {
            Hostname = hostname;
            HostPort = port;

            Socket = new TcpClient(Hostname, HostPort);
            Stream = Socket.GetStream();

            int bytesRead = Stream.Read(ReadBuffer, 0, 2);

            ProtocolVersion = BinaryParser.ToShort(ReadBuffer.Take(2).ToArray());

            return ProtocolVersion;
        }

        internal object ExecuteOperation<T>(T operation)
        {
            Request request = ((IOperation)operation).Request(SessionID);
            byte[] buffer;

            foreach (DataItem item in request.DataItems)
            {
                switch (item.Type)
                {
                    case "byte":
                    case "short":
                    case "int":
                        Send(item.Data);
                        break;
                    case "record":
                        buffer = new byte[2 + item.Data.Length];
                        Buffer.BlockCopy(BinaryParser.ToArray(item.Data.Length), 0, buffer, 0, 2);
                        Buffer.BlockCopy(item.Data, 0, buffer, 2, item.Data.Length);
                        Send(buffer);
                        break;
                    case "bytes":
                    case "string":
                    case "strings":
                        buffer = new byte[4 + item.Data.Length];
                        Buffer.BlockCopy(BinaryParser.ToArray(item.Data.Length), 0, buffer, 0, 4);
                        Buffer.BlockCopy(item.Data, 0, buffer, 4, item.Data.Length);
                        Send(buffer);
                        break;
                    default:
                        break;
                }
            }

            return ((IOperation)operation).Response(Receive());
        }

        private void Send(byte[] rawData)
        {
            if (Stream.CanWrite)
            {
                Stream.Write(rawData, 0, rawData.Length);
            }
        }

        private Response Receive()
        {
            Response response = new Response();
            IEnumerable<byte> buffer = new List<byte>();

            if (Stream.CanRead)
            {
                do
                {
                    int bytesRead = Stream.Read(ReadBuffer, 0, ReadBuffer.Length);

                    buffer = buffer.Concat(ReadBuffer);
                }
                while (Stream.DataAvailable);

                response.Data = buffer.ToArray();
            }

            return response;
        }
    }
}
