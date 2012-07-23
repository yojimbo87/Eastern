using System.Linq;
using System.Collections.Generic;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class DataClusterCount : IOperation
    {
        internal List<short> Clusters { get; set; }

        internal DataClusterCount()
        {
            Clusters = new List<short>();
        }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DATACLUSTER_COUNT) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });
            
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "short", Data = BinaryParser.ToArray((short)Clusters.Count) });

            foreach (short cluster in Clusters)
            {
                request.DataItems.Add(new DataItem() { Type = "short", Data = BinaryParser.ToArray(cluster) });
            }

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            long recordsCount = 0;

            if (response == null)
            {
                return recordsCount;
            }

            // operation specific fields
            recordsCount = BinaryParser.ToLong(response.Data.Skip(offset).Take(8).ToArray());
            offset += 8;

            return recordsCount;
        }
    }
}
