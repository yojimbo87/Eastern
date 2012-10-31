using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class Count : IOperation
    {
        internal string ClusterName { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.COUNT) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });

            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(ClusterName) });

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
