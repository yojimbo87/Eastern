using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class DataClusterAdd : IOperation
    {
        internal OClusterType Type { get; set; }
        internal string Name { get; set; }
        internal string Location { get; set; }
        internal string DataSegmentName { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DATACLUSTER_ADD) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(Type.ToString().ToUpper()) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(Name) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(Location) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(DataSegmentName) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            short clusterID = 0;

            // operation specific fields
            clusterID = BinaryParser.ToShort(response.Data.Skip(offset).Take(2).ToArray());
            offset += 2;

            return clusterID;
        }
    }
}
