using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class DataClusterDataRange : IOperation
    {
        internal short ClusterID { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DATACLUSTER_DATARANGE) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });

            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "short", Data = BinaryParser.ToArray(ClusterID) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            long[] dataRange = new long[2];

            if (response == null)
            {
                return dataRange;
            }

            // operation specific fields
            dataRange[0] = BinaryParser.ToLong(response.Data.Skip(offset).Take(8).ToArray());
            offset += 8;

            dataRange[1] = BinaryParser.ToLong(response.Data.Skip(offset).Take(8).ToArray());
            offset += 8;

            return dataRange;
        }
    }
}
