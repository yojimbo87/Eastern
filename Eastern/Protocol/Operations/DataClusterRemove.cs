using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class DataClusterRemove : IOperation
    {
        internal short ClusterID { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();
            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DATACLUSTER_REMOVE) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "short", Data = BinaryParser.ToArray(ClusterID) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            byte deleteOnClientSide = 0;

            // operation specific fields
            deleteOnClientSide = BinaryParser.ToByte(response.Data.Skip(offset).Take(1).ToArray());
            offset += 1;

            return deleteOnClientSide;
        }
    }
}
